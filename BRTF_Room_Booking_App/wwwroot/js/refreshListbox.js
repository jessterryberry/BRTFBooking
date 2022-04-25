//Reusable refresh routine for SelectLists
//Parameters: listbox_ID - ID of the Select to refresh
//  uri - /controller/action/... to call to get a new SelcectList as JSON
// showNoDataMsg - Boolean if you want to clear all options and show NO DATA message
//      if no data is returned.  Otherwise it leaves the original values in place.
// fadeOutIn - Boolean for visual effect after refresh
function refreshListbox(listbox_ID, URL, showNoDataMsg, noDataMsg, fadeOutIn) {
    var theListbox = $("#" + listbox_ID);
    $(function () {
        $.getJSON(URL, function (data) {
            if (data !== null && !jQuery.isEmptyObject(data)) {
                theListbox.empty();
                $.each(data, function (index, item) {
                    theListbox.append($('<option/>', {
                        value: item.value,
                        text: item.text
                    }));
                });
                theListbox.trigger("chosen:updated");
            } else {
                if (showNoDataMsg) {
                    theListbox.empty();
                    if (noDataMsg == null || jQuery.isEmptyObject(noDataMsg)) {
                        noDataMsg = 'No Matching Data'
                    };
                    theListbox.append($('<option/>', {
                        value: null,
                        text: noDataMsg
                    }));
                    theListbox.trigger("chosen:updated");
                }
            }
        });
    });
    if (fadeOutIn) {
        theListbox.fadeToggle(400, function () {
            theListbox.fadeToggle(400);
        });
    }
    return;
}