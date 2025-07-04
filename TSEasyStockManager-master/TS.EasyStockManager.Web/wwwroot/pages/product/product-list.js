﻿$(document).ready(function () {
    var datatable = $('#datatable').dataTable({
        "searching": false,
        "iDisplayLength": 10,
        "ordering": false,
        "lengthChange": false,
        "bServerSide": true,
        "processing": true,
        "paging": true,
        "sAjaxSource": "/Product/List",
        "info": true,
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            aoData.push(
                { "name": "returnformat", "value": "plain" },
                { "name": "ProductName", "value": $('input[name="ProductName"]').val() },
                { "name": "Barcode", "value": $('input[name="Barcode"]').val() },
                { "name": "CategoryId", "value": $('select[name="CategoryId"]').val() },
                { "name": "UnitOfMeasureId", "value": $('select[name="UnitOfMeasureId"]').val() }
            );
            $.ajax({
                "dataType": 'json',
                "type": "GET",
                "url": sSource,
                "data": aoData,
                "success": function (data) {
                    if (data.IsSucceeded == true) {
                        fnCallback(data);
                    }
                    else {
                        toastr.error(data.UserMessage);
                    }
                }
            });
        },
        aoColumns:
            [
                {
                    "sDefaultContent": "",
                    "bSortable": false,
                    "mRender": function (data, type, row) {
                        var img = '<img src="' + row.ImageDisplay + '" src height="60" />';
                        return img;
                    }
                },
                {
                    mDataProp: "ProductName"
                },
                {
                    mDataProp: "Barcode"
                },
                {
                    mDataProp: "Price",
                    mRender: function (data, type, row) {
                        if (typeof data === "string" && data.includes("₹")) {
                            return data;
                        }
                        return "₹" + parseFloat(data).toLocaleString('en-IN', { minimumFractionDigits: 2 });
                    }
                },
                {
                    mDataProp: "CategoryName"
                },
                {
                    mDataProp: "UnitOfMeasureName"
                },
                {
                    "sDefaultContent": "",
                    "bSortable": false,
                    "mRender": function (data, type, row) {
                        var buttons = "";
                        buttons += '<a href="/Product/Edit/' + row.Id + '" class="btn btn-xs btn-warning"><i class="fas fa-pen"></i> Edit</a>&nbsp;'
                        buttons += '<a onclick="deleteRow(this,' + row.Id + ')"  class="btn btn-xs btn-danger"><i class="fas fa-trash"></i> Delete</a>'
                        return buttons;
                    }
                }
            ]
    });

    $("#btnFilter").click(function () {
        datatable.fnFilter();
    });

    $("#btnClear").click(function () {
        $('div.dataTable-search-form').clearForm();
        datatable.fnFilter();
    });

    $('.enter-keyup').keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            datatable.fnFilter();
        }
    });

});

function deleteRow(row, id) {
    $.ajax({
        url: '/Product/Delete/' + id,
        type: "POST",
        async: false,
        success: function (data) {
            if (data.IsSucceeded) {
                var aPos = $('#datatable').dataTable().fnGetPosition(row);
                $('#datatable').dataTable().fnDeleteRow(aPos);
                toastr.success(data.UserMessage);
            }
            else {
                toastr.error(data.UserMessage);
            }
        }
    });
}
