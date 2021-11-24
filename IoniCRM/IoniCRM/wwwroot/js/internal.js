// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



// Testes com select picker //

let search = document.getElementById("search_word")
let items = document.getElementsByClassName("dropdown-item")

function buildDropDown(values) {
    let contents = []
    for (let name of values) {
        contents.push('<input type="button" class="dropdown-item rounded mt-1" value="' + name + '"/>')
    }
    $('#menuItems').append(contents.join(""))

    $('#empty').hide()
}

function filter(word) {
    let length = items.length
    //let collection = []
    let hidden = 0

    for (let i = 0; i < length; i++) {
        if (items[i].value.toLowerCase().startsWith(word)) {
            $(items[i]).show()
        }
        else {
            $(items[i]).hide()
            hidden++
        }
    }

    if (hidden === length) {
        $('#empty').show()
    }
    else {
        $('#empty').hide()
    }
}

$('#menuItems').on('click', '.dropdown-item', function () {
    $('#dropdown_select').text($(this)[0].value)
    $("#dropdown_select").dropdown('toggle');
})

//$('#dropdown_select').click(buildDropDown(names))

//buildDropDown(names)

