$(() => {
    console.log("ldaksjf")
    $("#new-contributor").on("click", function () {
        $("#contributor-modal").modal()
    })
    $(".edit-btn").on("click", function () {
        console.log("ldaksjf")
        var personId = $(this).data('person-id');
        var firstName = $(this).data('first-name');
        var lastName = $(this).data('last-name');
        var alwaysincluded = $(this).data('always-included');
        var date = $(this).data('date-created');
        var row = $(this).closest('tr');
        var cell = row.find('td:eq(2)').text();
        console.log(date);
        console.log(alwaysincluded);
        $("#contributor-id").val(personId);
        $("#contributor-first-name").val(firstName);
        $("#contributor-last-name").val(lastName);
        $("#contributor-cell").val(cell);
        $("#contributor-created-at").val(date);
        $('#contributor-always-included').prop('checked', alwaysincluded === "True");
        $('#deposit').hide();
        $("#contributor-form").attr("action", "/Contributors/EditContributer");
        $("#contributor-modal").modal();
    })

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

    $(".deposit-btn").on("click", function () {
        var personId = $(this).data('person-id');
        $("#person-id").val(personId);
        $("#deposit-modal").modal();
    })
})