﻿$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();

    var setValue = function (obj, name, value) {

        if (obj[name] && name.indexOf("[") != -1) {
            if (!obj[name].push) {
                obj[name] = [obj[name]];
            }
            obj[name].push(value || '');
        } else if (obj[name] == null) {
            obj[name] = value || '';
        }
    }

    $.each(a, function () {

        var names = this.name.split('.');

        var self = this;

        if (names.length > 1) {
            var obj = {};
            var lastObj = null;

            $.each(names, function (i, name) {
                if (i == 0) {
                    setValue(o, name, obj);
                }
                else if (i == names.length - 1) {
                    setValue(obj, name, self.value)
                }
                else {
                    if (lastObj == null)
                        lastObj = obj;

                    var tempObj = {};

                    setValue(lastObj, name, tempObj)

                    lastObj = tempObj;
                }

                setValue(o, "get_" + name, function () {
                    return this[name];
                });
            });
        } else {
            setValue(o, self.name, self.value)
            setValue(o, "get_" + self.name, function () {
                return this[self.name];
            });
        }
    });
    return o;
};

var getObjectFromPrefix = function (obj, prefix) {
    var innerPrefix = prefix != null ? prefix : "";
    var prefixes = innerPrefix.split('.');

    var tempObj = obj;

    $.each(prefixes, function () {
        tempObj = tempObj[this];
    });

    return tempObj;
}

FormValidator.getFormModel = function (formValidator) {
    var form = $('#' + formValidator.Form)
    var model = form.serializeObject()

    if (formValidator.Prefix == "")
        return model

    return getObjectFromPrefix(model, formValidator.Prefix)
}

FormValidator.getValueFromModel = function (property) {
    return function (model) {
        return model[property];
    }
}

FormValidator.setValueOnModel = function (value) {
    return function (property) {
        return function (model) {
            model[property] = value;
        }
    }
}

FormValidator.getRemoteValidationResult = function (remoteValidator) {
    return function (model) {
        var data = {}
        $.post(remoteValidator.url, data, function (result) {

        }, 'json');
    }
}