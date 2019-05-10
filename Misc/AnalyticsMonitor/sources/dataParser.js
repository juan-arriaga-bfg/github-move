/**
 * Created by keht on 23.12.2016.
 */

const _ = require('underscore');
const moment = require('moment');

let dataId = 0;

const setsList = [
    'flags',
    'userstats',
    'balances',
    'standart',
    'abtest',
    'story',
    'location'
];

const mainParamsList = [
    'eventName',
    'name',
    'type',
    'action',
];

function toTitleCase(str) {
    str = str.toLowerCase().split(' ');
    for (var i = 0; i < str.length; i++) {
        str[i] = str[i].charAt(0).toUpperCase() + str[i].slice(1);
    }
    return str.join(' ');
}

// Тут храним все поступившие данные (то что отправляет игра)
var data = [];

function add(url) {
    const id = ++dataId;
    data.push({
            id : id,
            data : formatItem(url, id)
        });
    console.log("New data recieved!")
}

// По запросу возвращаем номер и отформатированные в хтмл данные. Выдаем не весь запрос, а начиная с idToStart
function get(idToStart) {
    var ret = {
        count: data.length,
        data: []
    };

    if (!(idToStart < 0)) {
        for (var i = idToStart; i < data.length; i++) {
            ret.data.push(data[i]);
        }
    }

    return ret;
}

function replacer(match, pIndent, pKey, pVal, pEnd) {
        var key = '<span class=json-key>';
        var val = '<span class=json-value>';
        var str = '<span class=json-string>';
        var r = pIndent || '';
        if (pKey)
            r = r + key + pKey.replace(/[": ]/g, '') + '</span>: ';
        if (pVal)
            r = r + (pVal[0] == '"' ? str : val) + pVal + '</span>';
        return r + (pEnd || '');
    };

function prettyPrint(obj) {
    var jsonLine = /^( *)("[\w]+": )?("[^"]*"|[\w.+-]*)?([,[{])?$/mg;
    return JSON.stringify(obj, null, 3)
        .replace(/&/g, '&amp;').replace(/\\"/g, '&quot;')
        .replace(/</g, '&lt;').replace(/>/g, '&gt;')
        .replace(jsonLine, replacer);
};


// Парсим урл из игры и превращаем его в красивый хтмл. На выходе - один элемент в таблице.
function formatItem(data, id) {
    //data = "http://stackoverflow.com/" + "api/v1/2e784f89d0bf43b7b824607a5c4cae22/evt/?s=0&ts=1482491194&n=dialog_rave_stuck&v=0&l=1&st1=custom&st2=dialog&st3=closed&data=eyJHQUlEIjoic29tZV9nb29nbGVfYWR2ZXJ0aXNpbmdfaWQiLCJyYXZlSUQiOiI4OGYzN2JhOWJjZTc0YWM1ODQyN2MzMzViNDk0ODk1NyIsInRpbWVwbGF5ZWQiOiIxODgiLCJzdG9uZWJhbCI6IjAiLCJzZXNzaW9uSWQiOiI5MTEyYzZiYy02ZTRlLTQ2Y2EtOGQ1Yy1jY2Q2OTQ5NTI3MTciLCJiZmd1ZGlkIjoiMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMCIsImlzcGF5aW5nIjoiMCIsImlzY2hlYXQiOiIwIiwibGlmZWJhbCI6IjUiLCJwbGF0Zm9ybSI6ImFuZHJvaWQiLCJidWlsZCI6Ijk3IiwiYXBwVmVyc2lvbiI6IjEuMS4wIiwiYWJncm91cCI6IjEiLCJpc2ZiIjoiMCIsInByZW1pdW1iYWwiOiIwIiwiY3JiYWwiOiIwIiwibWF4bGV2ZWwiOiIxLTEiLCJsdGJhbCI6IjAiLCJqb2huYmFsIjoiMCJ9";
    const params = data['eventData'];
    const mainParams = {
        header: "Main Fields",
        eventName: params['details1'],
        name: params['name'] ? params['name'] : 'null',
        type: params['type'] ? params['type'] : 'null',
        action: params['action'] ? params['action'] : 'null'
    };

    const now = moment();
    const timestamp = {
        id: id + '</br>' + now.format("DD.MM.YYYY") + '</br>' + now.format("HH:mm:ss")
    };

    let ret = '';

    ret +=
        '<head>' +
        '<link rel="stylesheet" href="/stylesheets/table-entry.css" />' +
        '<link rel="stylesheet" href="/stylesheets/json-highlight.css" />' +
        '<script src="javascripts/rowTools.js"></script>' +
        '</head>';

    ret += openDiv(id);
    ret += openTable();
    ret += renderColumn(timestamp);
    ret += renderColumn(mainParams);
    ret += renderJsonSets(params);
    ret += closeTable();
    ret += addTools(id);
    ret += addBr();
    ret += addBr();
    ret += closeDiv();

    return ret;
}

// Парсим json sets
function renderJsonSets(jsonParams) {
    let ret = "";
    const generatedSets = {};

    // Собираем реальные значения из джейсона
    const keys = [];
    const values = [];

    for (let key in jsonParams) {
        if (jsonParams.hasOwnProperty(key)) {
            keys.push(key);
            values.push(jsonParams[key]);
        }
    }

    for (let i = 0; i < setsList.length; i++) {
        let key = setsList[i];
        generatedSets[key] = [{ header : toTitleCase(key)}];
    }

    generatedSets['Unknown'] = [{header : 'Other'}];

    // Пробегаемся по полям, ищем к какому сету они относятся и добавляем
    for (let i = 0; i < keys.length; i++) {
        const key = keys[i];
        const value = values[i];

        // const valueJsonStr = JSON.stringify(value, null, 1);
        //let valueHl = valueJsonStr.replace(/\n/g, '</br>').replace(/\n/g, '</br>;');


        let valueHl = prettyPrint(value).replace(/\n/g, '</br>').replace(/   /g, '&nbsp;&nbsp;');

        let item = {};
        item[key] = valueHl;

        if (setsList.includes(key)) {
            generatedSets[key].push(item);
        } else {
            if (!mainParamsList.includes(key)) {
                generatedSets['Unknown'].push(item);
            }
        }
    }

    // Генерим столбцы
    for (let key in generatedSets) {
        if (generatedSets.hasOwnProperty(key)) {
            ret += renderColumn(generatedSets[key]);
        }
    }

    return ret;
}

function openDiv(id) {
    return '<div id=' + id + '>';
}

function closeDiv() {
    return '</div>';
}

function openTable() {
    return '<table><tbody><tr>';
}

function closeTable() {
    return '</tr></tbody></table>';
}

// Тула по управлению строкой
function addTools(id) {
    return '<a onclick="rowToolsClick(' + id +');">remove row</a>';
}

function addBr(id) {
    return '</br>';
}

function padRight(text, len) {
    var ret = text;

    var startLen = text.length;
    var finalLen = len - startLen;

    while (finalLen > 0) {
        ret += '&nbsp;';
        finalLen -=1;
    }
    return ret;
}

// Строим один столбец
function renderColumn(data) {
    var result = '';
    var header = '';

    var keys = [];
    var values = [];
    var maxKeyLen = 0;

    // Раскладываем данные по массивам, находим хидер и длину ключа для выравнивания.

    // Данные могут прийти как массив
    if (_.isArray(data)) {
        for (var i = 0; i < data.length; i++) {
            var item = data[i];
            for (var key2 in item) {
                if (item.hasOwnProperty(key2)) {
                    keys.push(key2);
                    values.push(item[key2]);

                    if (key2 === 'header') {
                        header = item[key2];
                    }

                    if (key2.length > maxKeyLen) {
                        maxKeyLen = key2.length;
                    }
                }
            }
        }
    // ... а могут и как объект
    } else {
        header = data['header'];
        for (var key in data) {
            if (data.hasOwnProperty(key)) {
                keys.push(key);
                values.push(data[key]);
                if (key.length > maxKeyLen) {
                    maxKeyLen = key.length;
                }
            }
        }
    }

    result += '<td>';

    // Хидер плюс количество элементов
    if (header) {
        var len = keys.length - 1;
        result += '<b>(' + len + ') ' + header + ':</b></br>';
    }

    // Элементы
    for (var i = 0; i < keys.length; ++i) {
        var key = keys[i];
        var val = values[i];

        if (key == 'header') {
            continue;
        }

        result += padRight(key + ':', maxKeyLen + 1) + '&nbsp' + val + '</b></br>';
    }

    result += '</td>';
    return result;
}

exports.add = add;
exports.get = get;
exports.debug = formatItem;