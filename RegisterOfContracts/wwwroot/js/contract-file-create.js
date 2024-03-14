$(document).ready(function () {
    $('#generateButton').click(function () {
        var selectedPattern = $('#selectedPattern').val();
        var contractId = $('#id').val();

        // Показываем спиннер
        startGenerating();

        setTimeout(function ()
        {

            $.ajax({
                url: '/DocCreate/Index',
                type: 'POST',
                data: { contractId: contractId, selectedPattern: selectedPattern },
                success: function (response) {

                    //=======Настройка модального окна=======//
                    //Устанавливаем название документа
                    $('#exampleModalLabel').text(response.fileName); 

                    //Отобразить HTML контент
                    $('#fileContentArea').html(response.fileContent); //Помещаем содержимое документа в тело окна

                    //=======Настройка кнопки скачивания=======//

                    //Преобразование base64 строк в бинарные данные
                    var binaryData = atob(response.fileData);
                    var byteArray = new Uint8Array(binaryData.length);
                    for (var i = 0; i < binaryData.length; i++) {
                        byteArray[i] = binaryData.charCodeAt(i);
                    }
                    
                    //Создание Blob объекта и URL для скачивания файла
                    var blob = new Blob([byteArray], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' });
                    var fileURL = URL.createObjectURL(blob);

                    //Настройка кнопки
                    var downloadBut = document.getElementById('downloadFileBut');
                    downloadBut.href = fileURL;
                    downloadBut.download = response.fileName
                    
                    //Открываем модальное окно
                    $('#documentView').modal('show');

                    stopGenerating(); // Скрываем спиннер
                },
                error: function (response) {
                    alert("При создании файла, произошла непредвиденная ошибка!");
                    stopGenerating(); // Скрываем спиннер после ошибки
                }
            });

        }, 1000);
    });

    var intervalId;

    // Функция для замены текста на кнопке
    function startGenerating() {
        var button = $('#generateButton');
        button.text('Генерация файла');

        var dots = '.';
        intervalId = setInterval(function () {
            if (dots.length < 3) {
                dots += '.';
                button.text('Генерация файла' + dots);
            } else {
                dots = '.';
                button.text('Генерация файла' + dots);
            }
        }, 500);
    }

    // Функция для возврата исходного текста
    function stopGenerating() {
        clearInterval(intervalId);
        var button = $('#generateButton');
        button.text('Сформировать');
    }

});

