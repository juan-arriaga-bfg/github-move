/**
 * Created by keht on 04.04.2017.
 */

function rowToolsClick(id) {
    var element = document.getElementById(id);
    element.outerHTML = "";
    delete element;
}