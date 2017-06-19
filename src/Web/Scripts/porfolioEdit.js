


var table = document.getElementById('PortfolioTable');

$('#PortfolioTable').find('tr td:last-child').on('click', $(this).find('td:last-child'), function () {
    table.onclick = function (e) {
        e = e || event;
        var eventEl = e.srcElement || e.target,
            parent = eventEl.parentNode;
        //pull portfolio and benchmarks to edit panel
        //parent is just the portfolio row HTML between <tr..> and </tr>
        clickedEdit(parent);
    }
});

/************   Get    ************/
function clickedEdit(parent) {
    if (isPortfolio(parent) && confirm("Do you want to edit this porfolio?")) {
        makeOld(parent);

        //make editable form
        var row = '<tr style="background: lavenderblush">';
        for (var i = 0; i < parent.cells.length - 1; i++)
            row += `<td>${parent.cells[i].innerHTML}</td>`;
        row += '</tr>';
        //make Number and Name editable forms
        row = makeNumberEditable(row);
        row = makeNameEditable(row);

        var numRows = table.rows.length;

        for (var slot = parent.rowIndex + 1; slot < numRows; slot++) {  //benchmark is in the slot'th row
            if (!isPortfolio(table.rows[slot])) {
                var bench = '<tr>';
                for (var j = 0; j < table.rows[slot].cells.length; j++)     //j is the column
                    bench += '<td>' + table.rows[slot].cells[j].innerHTML + '</td>';
                bench += '</tr>';
                bench = makeBenchmarkEditable(bench, slot - parent.rowIndex);
                row += bench;
            } else {
                //stop looking for benchmarks when find another portfolio
                break;
            }
        }
        loadEdit(row);

    } else {
        throw "This should not be editable on its own.";
    }
}

function makeNumberEditable(row) {
    //find first and second <td>
    var firstStart = row.search("<td>");
    var firstEnd = row.search("</td>");
    var beginString = row.substr(0, firstStart + 4);
    var endString = row.substring(firstEnd, row.length);
    var number = row.substring(firstStart + 4, firstEnd);
    var newNumber = '<input type="text" id = "number" value = "' + number + '">';

    var newRow = beginString + newNumber + endString;
    return newRow;
}

function makeNameEditable(row) {
    var startPosition = getPosition(row, "<td>", 2);
    var endPosition = getPosition(row, "</td>", 2);
    var beginString = row.substr(0, startPosition + 4);
    var endString = row.substring(endPosition, row.length);
    var name = row.substring(startPosition + 4, endPosition);
    var newName = '<input type="text" id = "name" value = "' + name + '">';

    var newRow = beginString + newName + endString;
    return newRow;
}

function makeBenchmarkEditable(benchRow, b) {
    //no number in first column
    var benchNameStart = getPosition(benchRow, "<td>", 2);
    var benchNameEnd = getPosition(benchRow, "</td>", 2);
    var benchBegin = benchRow.substring(0, benchNameStart + 4);
    var benchEnd = benchRow.substring(benchNameEnd, benchRow.length);

    var benchName = benchRow.substring(benchNameStart + 4, benchNameEnd);
    var newBenchName = '<input type="text" id = "benchmark' + b + '" value = "' + benchName + '">';

    var newBenchRow = benchBegin + newBenchName + benchEnd;
    return newBenchRow;
}

function getPosition(string, subString, index) {
    return string.split(subString, index).join(subString).length;
}

function makeOld(parent) {
    var rowElement = document.getElementById("PortfolioTable").rows[parent.rowIndex];
    //have to override something
    rowElement.removeAttribute('style');
    rowElement.style.background = '#ffa4c2';
    //benchmarks
    var numRows = table.rows.length;
    for (var slot = parent.rowIndex + 1; slot < numRows; slot++) {  //benchmark is in the slot'th row
        if (!isPortfolio(table.rows[slot])) {
            var diff = (table.rows[parent.rowIndex].cells.length - table.rows[slot].cells.length);
            //make sure  correct number of cells
            if (diff > 0) {
                for (let k = 0; k < diff; k++) {
                    $("#changable").text(table.rows[slot]);
                    table.rows[slot].insertCell(-1);
                }
                $("#changable2").text(table.rows[slot].cells);
            }
            table.rows[slot].style.background = '#ffa4c2';

        } else {
            //stop looking for benchmarks when find another portfolio
            break;
        }
    }


}

function loadEdit(row) {
    $('#putResult').after(row);
    $(function () {
        $('.myClick').css('pointer-events', 'none');
    });
    $('.myClick').text('');
    $('button').css('display', 'initial');
    $('.form-inline').css('display', 'none');
}

/************   Post    ************/

function updateDatabase() {
    //$("#changable2").text(document.getElementById('name').value);
    var portfolioName = document.getElementById('name').value;
    var portfolioNumber = document.getElementById('number').value;

    var portfolioId = new Array();
    portfolioId['name'] = portfolioName;
    portfolioId['number'] = portfolioNumber;

    var numRows = document.getElementById('PortfolioTable').rows.length;
    var benchmarkIds = new Array();
    //put benchmarks into benchmarkIds
    for (var i = 1; i < numRows; i++) {
        var id = "benchmark" + i;
        try {
            var value = document.getElementById(id).value;
            benchmarkIds[id] = value;
        } catch (e) {
            break;
        }
    }


    var myPorts = '';
    var myBenchs = '';
    for (key in portfolioId) {
        myPorts += "key " + key + ": " + portfolioId[key] + " ~ ";
    }
    for (key in benchmarkIds) {
        myBenchs += "key " + key + ": " + benchmarkIds[key] + " ~ ";
    }
    $("#changable").text(myPorts);
    $("#changable2").text(myBenchs);
}







/************   Other    ************/
$(function () {
    $('th').css('pointer-events', 'none');
});
$(function () {
    $('td').css('pointer-events', 'none');
});
$(function () {
    $('.myClick').css('pointer-events', 'auto');
});

function isPortfolio(element) {
    return (element.cells[0].innerHTML != '');
}

