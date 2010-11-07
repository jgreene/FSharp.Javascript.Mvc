var getObjectFromArray = function (array) {
    var o = {};
    var a = array;

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
    return function (onCompleteValidation) {
        var form = $('#' + formValidator.Form)
        var array = form.serializeArray()

        var filteredArray = (function () {
            var arr = []
            var startsWith = function (input, toMatch) {
                return input.substr(0, toMatch.length) === toMatch
            }
            var removePrefix = function (input) {
                var name = (function () {
                    if (formValidator.Prefix == "")
                        return input.name;

                    return input.name.substring((formValidator.Prefix.length + 1), input.name.length)

                })();

                return { name: name, value: input.value }

            }
            $.each(array, function () {
                if (startsWith(this.name, formValidator.Prefix)) {
                    var item = removePrefix(this);
                    arr.push(item)
                }
            })

            return arr;
        })();

        var result = getObjectFromArray(filteredArray);

        result.onCompleteValidation = onCompleteValidation

        return result
    }
}

FormValidator.getValueFromModel = function (model) {
    return function (property) {
        return model[property];
    }
}

FormValidator.setValueOnModel = function (model) {
    return function (property) {
        return function (value) {
            model[property] = value;
        }
    }
}

FormValidator.getRemoteValidationResult = function (model) {
    return function (remoteValidator) {
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
                if (result != null && result.Value != null) {
                    model.onCompleteValidation(remoteValidator.errorField)(result.Value)
                }
            }
        });

        return new Microsoft.FSharp.Core.FSharpOption.None();
    }
}