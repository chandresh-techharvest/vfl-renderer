/* Place your scripts here */


/* Table Search */
//$(document).ready(function () {

//    $('.table-responsive').each(function (index, value) {
//        var headerCount = $(this).find('thead th').length;

//        for (i = 0; i <= headerCount; i++) {
//            var headerLabel = $(this).find('thead th:nth-child(' + i + ')').text();

//            $(this).find('tr td:not([colspan]):nth-child(' + i + ')').replaceWith(
//                function () {
//                    return $('<td data-label="' + headerLabel + '">').append($(this).contents());
//                }
//            );
//        }

//    });
//    $('.table-responsive table').each(function (index, value) {
//        var headerCount = $(this).find('thead th').length;

//        for (i = 0; i <= headerCount; i++) {
//            var headerLabel = $(this).find('thead th:nth-child(' + i + ')').text();

//            $(this).find('tr td:not([colspan]):nth-child(' + i + ')').replaceWith(
//                function () {
//                    return $('<td data-label="' + headerLabel + '">').append($(this).contents());
//                }
//            );
//        }

//    });

//    $(".couf").change(function () {
//        $(this).css("background-color", "#D6D6FF");
//        alert($(".country-ddl").val());

//        var value = $(this).val().toLowerCase();
//        $(".col-lg-12").filter(function () {
//            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
//        });

//    });

//    $("#myInput").on("keyup", function () {
//        var value = $(this).val().toLowerCase();

//        $("#myTable tbody tr").filter(function () {
//            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
//        });

//    });
//});

/* For Table Search */
