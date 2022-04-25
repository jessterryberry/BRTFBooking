$('#btnRight').click(function (e) {
    var selectedOpts = $('#selectedOptions option:selected');
    if (selectedOpts.length == 0) {
        alert("Nothing to move.");
        e.preventDefault();
    }

    $('#availOptions').append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
});

$('#btnLeft').click(function (e) {
    var selectedOpts = $('#availOptions option:selected');
    if (selectedOpts.length == 0) {
        alert("Nothing to move.");
        e.preventDefault();
    }

    $('#selectedOptions').append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
});

$(document).on("click", "#btnSubmit", function (e) {
    $('#selectedOptions option').prop('selected', true);
});

////let DDLforChosen = document.getElementById("selectedOptions");
////let DDLforAvail = document.getElementById("availOptions");

/////*function to switch list items from one ddl to another
////use the sender param for the DDL from which the user is multi-selecting
////use the receiver param for the DDL that gets the options*/
////function switchOptions(event, senderDDL, receiverDDL) {
////    //find all selected option tags - selectedOptions becomes a nodelist 
////    let senderID = senderDDL.id;
////    let selectedOptions = document.querySelectorAll(`#${senderID} option:checked`);
////    event.preventDefault();

////    if (selectedOptions.length === 0) {
////        alert("Select options from the list below to move them.");
////    }
////    else {
////        selectedOptions.forEach(function (o, idx) {
////            senderDDL.remove(o.index);
////            receiverDDL.appendChild(o);
////                if ($("#selectedOptions option").length == 0) {
////                    console.log($("#selectedOptions option").length)
////                    $("#selectedOptionsError").html("You must select at least one option from the right.");
////                }
////                else {
////                    $("#selectedOptionsError").html("");
////                }
////        });
////    }
////}
//////create closures so that we can access the event & the 2 parameters
////let addOptions = (event) => switchOptions(event, DDLforAvail, DDLforChosen);
////let removeOptions = (event) => switchOptions(event, DDLforChosen, DDLforAvail);
//////assign the closures as the event handlers for each button
////document.getElementById("btnLeft").addEventListener("click", addOptions);
////document.getElementById("btnRight").addEventListener("click", removeOptions);

////document.getElementById("btnSubmit").addEventListener("click", function () {
////    DDLforChosen.childNodes.forEach(opt => opt.selected = "selected");
////})