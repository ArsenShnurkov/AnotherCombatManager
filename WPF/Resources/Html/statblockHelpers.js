﻿var Combatant = ko.ViewModel.extend({
    
});

var Character = Combatant.extend({
});

var Monster = Combatant.extend({
});

var Browser = {
    Version: function() {
        var version = 999; // we assume a sane browser
        if (navigator.appVersion.indexOf("MSIE") != -1) {
            // bah, IE again, lets downgrade version number
            version = parseFloat(navigator.appVersion.split("MSIE")[1]);
        }
        return version;
    }
}

var StatblockHelpers = {
    toCommaNum: function (num, signed) {
        var sign = signed ? (num > 0 ? '+' : '') : '',
            regex = /(\d+)(\d{3})/,
            text = num + '';

        while (regex.test(text)) {
            text = text.replace(regex, '$1' + ',' + '$2');
        }

        return sign + text;
    },

    splitOnCaps: function (text) {
        var regex = /(?!^)(?=[A-Z])/,
            text = text + '';
        return text.replace(regex, " ");
    }
}

