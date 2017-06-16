function UpdatePortfolio(portfolio) {


    $.ajax({
        url: 'http://localhost:52899/api/UpdatePortfolio',
        type: 'POST',
        data: JSON.stringify(portfolio),
        dataType: "json",
        success: function(data) {
            alert("worked");
        },
        error: function(data) {
            alert("error");
        }
    });
}

function createPortfolio(name, number, benchmarks) {
    portfolio = {
        Name: name,
        Number: number,
        Benchmarks: benchmarks
    }

    return portfolio;
}




}