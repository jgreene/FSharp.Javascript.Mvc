$.fn.serializeObject = function () {
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

//onCompleteValidation is a 
FormValidator.getFormModel = function (onCompleteValidation) {
    return function (formValidator) {
        var form = $('#' + formValidator.Form)
        var model = form.serializeObject()

        var result = null

        if (formValidator.Prefix == "")
            result = model
        else
            result = getObjectFromPrefix(model, formValidator.Prefix)

        result.onCompleteValidation = onCompleteValidation

        return result
    }
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
        $.each(remoteValidator.arguments, function () {
            var arg = this;
            var methodArg = arg.Item1;
            var modelArg = arg.Item2;

            data[methodArg] = model[modelArg];
        });

        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: remoteValidator.url,
            data: data,
            success: function (result) {
                if (result.Value != null) {
                    model.onCompleteValidation(remoteValidator.errorField)(result.Value)
                }
            }
        });
        //        var result = $.ajax({
        //            global: false,
        //            async: false,
        //            type: 'POST',
        //            dataType: 'json',
        //            url: remoteValidator.url,
        //            data: data,
        //            complete: function (result) {

        //            }
        //        }).responseText;

        //        result = $.parseJSON(result);


        //        if (result.Value != null) {
        //            return new Microsoft.FSharp.Core.FSharpOption.Some(result.Value);
        //        }

        return new Microsoft.FSharp.Core.FSharpOption.None();
    }
}