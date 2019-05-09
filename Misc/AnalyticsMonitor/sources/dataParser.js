/**
 * Created by keht on 23.12.2016.
 */

var url = require("url");
const querystring = require('querystring');
var _ = require('underscore');

// Keep in sync with RobinHoodPuzzle\Assets\Scripts\RobinHood\BigFish\Analytics\AnalyticsController.cs
var sets = {
    BasicSet: [
        'BFGUDID',
        'raveID',
        'googleAdvertisingId',
        'androidId',
        'platform',
        'appVersion',
        'appBuildVersion',
        'sessionID',
        'playSessionId',
    ],
    PlayerSet: [
        'userlvl',
        'maxlevel',
        'timeplayed',
        'ispaying',
        'isfb',
        'abgroup',
        'ischeat',
    ],
    EconomySet: [
        'premiumbal',
        'itemid',
        'sourceid',
        'tierid',
        'txnid',
        'crbal',
        'johnbal',
        'ltbal',
        'stonebal',
        'lifebal',
        'item_name',
        'bundle_id',
        'free_boost',
    ],
    ProgressionSet: [
        'boost',
        'restart',
        'abandon',
        'extra_moves',
        'req_build',
        'req_collect',
        'req_score',
        'req_moves',
        'req_free_cells',
        'storage_usage',
        'cr_inv',
        'lt_inv',
        'john_inv',
        'lvl_type',
        'sheriff_attacks',
        'blackknight_attacks',
        'lighting_on_bomb_count',
        'defuse_bomb_count',
    ],
    SocialSet: [
        'login_channel'
    ],
};

// Keep in sync with RobinHoodPuzzle\Assets\Scripts\RobinHood\BigFish\Analytics\AnalyticsController.cs
var setsList = [
    'BasicSet',
    'PlayerSet',
    'EconomySet',
    'ProgressionSet',
    'SocialSet',
    'Unknown'
];

// Тут храним все поступившие данные (то что отправляет игра)
var data = [];

function add(url) {
    data.push({
            id : data.length,
            data : formatItem(url, data.length)
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

// Парсим урл из игры и превращаем его в красивый хтмл. На выходе - один элемент в таблице.
function formatItem(data, id) {
    //data = "http://stackoverflow.com/" + "api/v1/2e784f89d0bf43b7b824607a5c4cae22/evt/?s=0&ts=1482491194&n=dialog_rave_stuck&v=0&l=1&st1=custom&st2=dialog&st3=closed&data=eyJHQUlEIjoic29tZV9nb29nbGVfYWR2ZXJ0aXNpbmdfaWQiLCJyYXZlSUQiOiI4OGYzN2JhOWJjZTc0YWM1ODQyN2MzMzViNDk0ODk1NyIsInRpbWVwbGF5ZWQiOiIxODgiLCJzdG9uZWJhbCI6IjAiLCJzZXNzaW9uSWQiOiI5MTEyYzZiYy02ZTRlLTQ2Y2EtOGQ1Yy1jY2Q2OTQ5NTI3MTciLCJiZmd1ZGlkIjoiMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMCIsImlzcGF5aW5nIjoiMCIsImlzY2hlYXQiOiIwIiwibGlmZWJhbCI6IjUiLCJwbGF0Zm9ybSI6ImFuZHJvaWQiLCJidWlsZCI6Ijk3IiwiYXBwVmVyc2lvbiI6IjEuMS4wIiwiYWJncm91cCI6IjEiLCJpc2ZiIjoiMCIsInByZW1pdW1iYWwiOiIwIiwiY3JiYWwiOiIwIiwibWF4bGV2ZWwiOiIxLTEiLCJsdGJhbCI6IjAiLCJqb2huYmFsIjoiMCJ9";
    var parsedUrl = url.parse(data);
    var params = querystring.parse(parsedUrl.query);

    var mainParams = {
        header: "Fields",
        st1: params['st1'],
        st2: params['st2'],
        st3: params['st3'],
        n: params['n'],
        v: params['v'],
        l: params['l'],
    };

    var timestamp = {
        id: id + '</br>' + timeConverter(params['ts'])
    };

    var b64string = params['data'];
    var decodedData = Buffer.from(b64string, 'base64');
    var jsonParams = JSON.parse(decodedData);

    var ret = '';

    ret += '<head></head><link rel="stylesheet" href="/stylesheets/table-entry.css" /><script src="javascripts/rowTools.js"></script></head>';
    ret += openDiv(id);
    ret += openTable();
    ret += renderColumn(timestamp);
    ret += renderColumn(mainParams);
    ret += renderJsonSets(jsonParams);
    ret += closeTable();
    ret += addTools(id);
    ret += addBr();
    ret += addBr();
    ret += closeDiv();

    return ret;
}

// Парсим json sets
function renderJsonSets(jsonParams) {
    var ret = "";
    var generatedSets = {};

    // Собираем реальные значения из джейсона
    var keys = [];
    var values = [];

    for (var key in jsonParams) {
        if (jsonParams.hasOwnProperty(key)) {
            keys.push(key);
            values.push(jsonParams[key]);
        }
    }

    // Инициализируем выходной массив  // BasicSet, PlayerSet ...
    for (var predefinedSetKey in sets) {
        if (sets.hasOwnProperty(predefinedSetKey)) {
            generatedSets[predefinedSetKey] = [];
            generatedSets[predefinedSetKey].push({ header : predefinedSetKey});
        }
    }
    generatedSets['Unknown'] = [];
    generatedSets['Unknown'].push({header : 'Unknown'});

    // Пробегаемся по полям, ищем к какому сету они относятся и добавляем
    for (var i = 0; i < keys.length; i++) {
        var key = keys[i];
        var value = values[i];
        var item = {};
        item[key] = value;

        var isAssigned = false;
        for (var predefinedSetKey in sets) {   // BasicSet, PlayerSet ...
            if (sets.hasOwnProperty(predefinedSetKey)) {
                var predefinedValues = sets[predefinedSetKey];

                if (_.contains(predefinedValues, key)) {
                    if (!generatedSets[predefinedSetKey]) {
                        generatedSets[predefinedSetKey] = [];
                        generatedSets[predefinedSetKey].push({ header : predefinedSetKey});
                    }
                    generatedSets[predefinedSetKey].push( item );

                    isAssigned = true;
                    break;
                }
            }
        }

        // Не определенные ни в одну категорию айтемы попадают сюда
        if (!isAssigned) {
            generatedSets['Unknown'].push( item );
        }
    }

    // Генерим столбцы
    for (var key in generatedSets) {
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

                    if (key2 == 'header') {
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


function timeConverter(UNIX_timestamp){
    var a = new Date(UNIX_timestamp * 1000);
    var months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
    var year = a.getFullYear();
    var month = months[a.getMonth()];
    var date = a.getDate();
    var hour = a.getHours() >= 10 ? a.getHours() : '0' + a.getHours();
    var min  = a.getMinutes() >= 10 ? a.getMinutes() : '0' + a.getMinutes();
    var sec  = a.getSeconds() >= 10 ? a.getSeconds() : '0' + a.getSeconds();
    var time = /*date + ' ' + month + ' ' + year + ' ' + */hour + ':' + min + ':' + sec ;
    return time;
}

exports.add = add;
exports.get = get;
exports.debug = formatItem;