$(document).ready(function () {
    /*============================Механизм отображения фильтра=============================*/
    var filterState = Cookies.get('filter_state'); //Получаем данные Cookies
    if (filterState === undefined) {
        filterState = "Off"; // Значение по учолчанию
    }
    else
    {
        filterState = JSON.parse(Cookies.get('filter_state'));
    }

    var filterOn = document.getElementById("filterOn");
    var filterOff = document.getElementById("filterOff");
    //Проставляем начальное состояние индикатора фильтра в зависимости от данных Cookies
    if (filterState === "On") {
        filterOff.style.display = "block";
        filterOn.style.display = "none";
    } else
    {
        filterOff.style.display = "none";
        filterOn.style.display = "block";
    }
    /*============================Механизм сокрытия избираемых столбцов=============================*/
    var hiddenColumns = Cookies.get('hidden_columns'); //Получаем данные Cookies
    if (hiddenColumns === undefined) { //Если пусто, то наполняем значением по умолчанию
        hiddenColumns = [];
        hiddenColumnsDefault = [false, false, false, true, true, true, false, true, false, false]; //Здесь все столбцы, кроме id
        for (var i = 0; i < 10; i++) {
            hiddenColumns.push(hiddenColumnsDefault[i]);
        }
        Cookies.set('hidden_columns', JSON.stringify(hiddenColumns));
    }
    else {
        hiddenColumns = JSON.parse(Cookies.get('hidden_columns')); //Если есть, то сохраняем
    }
    let ul = document.createElement('ul');

    /*============================Метод переключения видимости фильтра=============================*/
    $('#filterArea').on('click', function () {
        var filterOn = document.getElementById("filterOn");
        var filterOff = document.getElementById("filterOff");

        var searchCols = document.querySelectorAll("#searchCol")


        if (filterOff.style.display === "none") {   //Включаем фильтр

            filterOff.style.display = "block";
            filterOn.style.display = "none";

            searchCols.forEach(function (element) { //Открываем окна поиска фильтра
                element.style.display = "block";
            });

            filterState = "On";
            Cookies.set('filter_state', JSON.stringify(filterState)); //Сохраняем состояние в Cookies

        } else {                                   //Выключаем фильтр

            filterOff.style.display = "none";
            filterOn.style.display = "block";

            searchCols.forEach(function (element) { //Скрываем окна поиска фильтра
                element.style.display = "none";
            });

            filterState = "Off";
            Cookies.set('filter_state', JSON.stringify(filterState)); //Сохраняем состояние в Cookies
        }
    });

    /*======================Метод заполнения модального окна settingsTable=====================*/
    $('#contract-table thead tr').clone(true).appendTo('#contract-table thead');
    $('#contract-table thead tr:eq(0) th').each(function (i) {
        let title = $(this).text();
        if (i !== 0) {
            if (hiddenColumns[i - 1] === true) {
                $('<li class="list-unstyled col-check-li">' +
                    '<label class="form-check-label"><input class="me-2 form-check-input checkCol" type="checkbox" value="' + i + '"/>' +
                    title +
                    '</label></li>').appendTo($(ul));
            } else {
                $('<li class="list-unstyled col-check-li">' +
                    '<label class="form-check-label"><input class="me-2 form-check-input checkCol" type="checkbox" value="' + i + '" checked/>' +
                    title +
                    '</label></li>').appendTo($(ul));
            }
        }
        /*-----------------------------А также основа механизма фильтрации-----------------------------*/
        if ([0].includes(i)) {
            $(this).html("");
            return;
        }

        //Начальное состояние фильтров
        if (filterState === "On") {
            $(this).html('<input id = "searchCol" class="form-control mb-3" style="width: 100%;" type="text" placeholder="' + title + '" />');
        } else
        {
            $(this).html('<input id = "searchCol" class="form-control" style="width: 100%; display:none" type="text" placeholder="' + title + '" />');
        }

        $('input', this).on('keyup change', function () {

            var $this = $(this);
            var delay = 500;

            clearTimeout($this.data('timer'));

            $this.data('timer', setTimeout(function () {

                $this.removeData('timer');

                if (table.column(i).search() !== $this.value) {
                    table
                        .column(i)
                        .search($this[0].value)
                        .draw();
                }

            }, delay));

        });

        /*--------------------------------------------------------------------------------------------*/
    });

    $(ul).appendTo($('#tableCols'));

    let checkboxes = document.getElementsByClassName('checkCol');
    //let checkboxes = document.getElementsByClassName('form-check-input');
    for (let i = 0; i < checkboxes.length; ++i) {
        checkboxes[i].addEventListener('change', function (ev) {

            hiddenColumns[i] = !ev.target.checked;
            Cookies.set('hidden_columns', JSON.stringify(hiddenColumns))

            table.columns([ev.target.value]).visible(ev.target.checked);
        });
    }

    /*============================Отображение таблицы контрактов=============================*/
    let table = $('#contract-table').DataTable
    (
        {
            stateSave: false,       //Если true, то состояние таблицы будет восстанавливаться при перезагрузке
            responsive: true,       //Если true, таблица становится адаптивной
            orderCellsTop: false,   //Если true, то сортировка будет применяться к верхнему ряду ячеек
            scrollX: true,
            scrollY: true,       // Установите высоту для таблицы, если нужно
            scrollCollapse: true,
            fixedHeader: {          //Задает горизонтальную плавающую прокрутку таблицы
                header: true,
                footer: false
            },       
            order: [[2, "asc"]],    //Определяет стандартный порядок сортировки по умолчанию для таблицы
            processing: true,       //Если true, то будет отображаться индикатор обработки (например, спиннер)
            language: {             //Определяет языковые настройки для таблицы
                url: "/js/russian.json"
            },
            serverSide: true,       //Если true, то таблица будет запрашивать данные у сервера по мере необходимости
            dom: '<"pull-сenter"B>rt<"row pt-3 align-items-baseline"<"col-sm-4"l><"col-sm-3"i><"col-sm-5"p>>', //Определяет расположение и элементы управления для отображения таблицы
            ajax: {                 //Задает параметры и URL для загрузки данных таблицы с сервера
                url: "/Contract/GetContractAjax",
                type: "POST",
                data: function (d) { d.hiddenColumns = hiddenColumns; },
                dataType: "json"
            },
            columnDefs: [           //Определяет стилистику для каждого столбца
                { className: "align-middle dt-center", targets: "_all" },
                { width: "100%", targets: [6] }, //Делаю столбец "Предмет контракта" больше
                { width: "100%", targets: [10] }, //Делаю столбец "Юр адрес поставщика" больше
            ],
            columns:                //Определяет настройки для каждого столбца таблицы
                [
                    { data: "id", name: "id", visible: false },
                    { data: "number", name: "number" },
                    {
                        data: "signDate", name: "signDate", render: function (data, type, full) {
                            if (data != null) {
                                return data.split("T")[0].split('-').reverse().join('.');
                            }
                        }
                    },
                    { data: "shortName", name: "shortName" },
                    { data: "purchaseCode", name: "purchaseCode" },
                    { data: "contractSubject", name: "contractSubject" },
                    { data: "contactEMail", name: "contactEMail" },
                    { data: "inn", name: "INN" },
                    { data: "kpp", name: "KPP" },
                    { data: "regNum", name: "regNum" },
                    { data: "address", name: "address" }

                ],
            initComplete: function () {
                //Настройка полосы прокрутки
                $('body').find('.dataTables_scrollBody').addClass("scrollbar");

                /*========================Метод возвращения столбцов отображения по умолчанию=========================*/
                $('#colToDefault').on('click', function () {
                    Cookies.remove("hidden_columns");   //Удаляем Cookies
                    location.reload();                  //Перезагружаем страницу
                });
                
                /*============================Обработчик события нажатия на кнопку "Поиск"=============================*/
                function performSearch() {
                    // Получаем значение из поля ввода
                    const searchValue = $('#search-label').val();

                    // Применяем поиск по значению в DataTables
                    table.search(searchValue).draw();
                }
                //Выполняем поиск по нажатию на enter
                document.addEventListener('keydown', function (event) {
                if (event.key === 'Enter' && event.target.id === 'search-label') {
                        performSearch();
                    }
                });

                $('#search-but').on('click', function () {
                    performSearch();
                });

                $('#contract-table tbody').on('click', 'tr', function () {
                    $('#contract-table tbody').on('click', 'tr', function () {
                        // Удаляем предыдущее выделение
                        $('.green-outline').removeClass('green-outline');
                        $('.bg-color-str-green').removeClass('bg-color-str-green');

                        // Добавляем класс к текущей строке, чтобы выделить ее
                        $(this).addClass('green-outline');
                        $(this).addClass('bg-color-str-green');
                    });
                });

                /*======================Обработчик модального окна с отображение параметров контракта=====================*/
                $('#contract-table tbody').on('dblclick', 'tr', function () {
                    // Отображаем данные строки в модальном окне
                    $.ajax({
                        url: '/Contract/GetContractByID', // Путь к контроллеру и методу
                        method: 'POST',
                        dataType: "json",
                        data: { id: table.row(this).data().id },
                        success: function (response) {
                            var data = response.data;
                            // Добавление данных на страницу
                            $('#id').val(data.id);
                            $('#number').text(data.number);
                            $('#signDate').text(function (signDate, type, full) {
                                if (data.signDate != null) {
                                    return data.signDate.split("T")[0].split('-').reverse().join('.');
                                }
                            });
                            $('#shortName').text(data.shortName);
                            $('#singularName').text(data.singularName);
                            $('#mailingAddress').text(data.mailingAddress);
                            $('#fullName').text(data.fullName);
                            $('#purchaseCode').text(data.purchaseCode);
                            $('#contractSubject').text(data.contractSubject);
                            $('#contactEMail').text(data.contactEMail);
                            $('#INN').text(data.inn);
                            $('#KPP').text(data.kpp);
                            $('#regNum').text(data.regNum);
                            $('#linkDocument').attr('href', data.linkDocument);
                            $('#address').text(data.address);
                            $('#counterpartyName').text(data.counterpartyName);
                            $('#href').attr('href', data.href);

                            AddAttachments(data.attachments);

                            $('#contractInfo').modal('show');
                        },
                        error: function () {
                            alert("При отображении данных контракта, произошла непредвиденная ошибка!");
                        }
                    });

                });

                /*======================Сокрытие столбцов=====================*/
                for (var i = 0; i < 10; i++) {
                    table.columns(i + 1).visible(!hiddenColumns[i]);
                }
            }
            
        }
    );

    function AddAttachments(attachments) {
        // Получаем область, куда будем добавлять строки
        let attachArea = document.getElementById('attachArea');

        attachArea.innerHTML = '';
        // Проверяем наличие объектов в attachments
        if (attachments.length > 0) {
            attachments.forEach(attachment => {

                // Создаем элемент строки для каждого объекта
                let attachmentElement = document.createElement('div');
                attachmentElement.innerHTML = `<a href= "${attachment.url}">
                                                    ${GetSVGImg(attachment.fileName)}
                                                    ${TrimFileName(attachment.fileName, 40)}
                                                </a>`;

                // Добавляем элемент строки в область attachArea
                attachArea.appendChild(attachmentElement);
            });
        } else {
            // Если список attachments пустой, выводим соответствующее сообщение
            attachArea.innerHTML = 'Пусто';
        }
    }

    function TrimFileName(fileName,maxLength)
    {
        if (fileName.split('.')[0].length <= maxLength) {
            return fileName;
        }

        let fileExtension = fileName.split('.').pop();
        let fileNameWithoutExtension = fileName.slice(0, -(fileExtension.length + 1)); // Удаляем расширение файла + точку
        let trimmedFileName = fileNameWithoutExtension.substring(0, maxLength - 3) + "..."; // Обрезаем до maxLength - 3 символов и добавляем "..."

        return trimmedFileName + '.' + fileExtension;
    }

    function GetSVGImg(fileName)
    {
        if (fileName.endsWith(".docx")) {
            return `<img src="/img/docx.svg">`;
        } else if (fileName.endsWith(".pdf")) {
            return `<img src="/img/pdf.svg">`;
        } else {
            return `<img src="/img/docx.svg">`;
        }
    }
});



