$(() => {
    $("#clear").on("click", function () {
        $("#search").val("")
    });


    $(document).ready(function () {
        var $rows = $("#my-table tr"), $cells = $rows.children();
        $("#search").keyup(function () {
            var term = $(this).val()
            if (term != "") {
                $rows.hide();
                $cells.filter(function () {
                    return $(this).text().toLowerCase().indexOf(term) > -1;
                }).parent("tr").show();
            } else {
                $rows.show();
            }
        });
    });
});