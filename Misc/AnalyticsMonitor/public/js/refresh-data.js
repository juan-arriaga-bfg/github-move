/**
 * Created by keht on 23.12.2016.
 */

// Храним локально айдишник последней полученной записи
var lastId = -1;

// Вызывается по таймеру
function refresh() {

    var isQueryInProgress = false;  // Не запускаем несколько запросов параллельно

    setTimeout(function () {
        if (!isQueryInProgress) {
            console.log("Refresh...");
            isQueryInProgress = true;

            // Отправляем запрос
            $.ajax({
                url: 'http://127.0.0.1:3000/data?id=' + lastId,
                method: 'GET'
            }).then(function (response) {
                // Если есть данные, добавляем в таблицу
                if (response['data'].length > 0) {
                    var out = '<div>';
                    for (var i = 0; i < response['data'].length; ++i) {
                        var text = response['data'][i];
                        out += formatItem(text);
                    }
                    out += '</div>';

                    $('body').append(out);

                    // И скроллим до конца страницы
                    document.body.scrollTop = document.body.scrollHeight - document.body.clientHeight;

                    console.log("Data recieved: " + JSON.stringify(response));
                }

                // Обновляем счетчик, чтобы в следующий раз запросить только то что надо, а не всю историю.
                lastId = response['count'];

                isQueryInProgress = false;

            }).catch(function (err) {
                console.error(err);
                isQueryInProgress = false;
            });
        }
        refresh();
    }, 1000);
}

// Стартуем бесконечный опрос
$(document).ready(function(){
    document.write('Waiting for data...' + '</br></br>');
    refresh();
});


function formatItem(data) {
    var ret = '';
    ret += (data['data']);//  + '</br>');
    return ret;
}

