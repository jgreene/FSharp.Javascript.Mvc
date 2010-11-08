var getObjectFromArray = function (array) {
    var result = {}

    var isArray = function (name) {
        return name != null && name.indexOf("[") != -1
    }

    var getArrayName = function (name) {
        return name.substring(0, name.indexOf("["))
    }

    var getArrayIndex = function (name) {
        return name.substring(name.indexOf("[") + 1, name.indexOf("]"))
    }

    var setGetter = function (name, obj) {
        obj["get_" + name] = function () {
            return this[name];
        }
    }

    var loop = function (index, name, lastName, lastObject, arrayItem, names) {

        var isLast = index == (names.length - 1)

        //the lastObject is an array. we will set the value to the array's index
        if (isArray(lastName)) {
            var index = getArrayIndex(lastName)

            var temp = lastObject[index]
            if (!temp) { temp = {}; }

            //this is the end of the names, this value will be the actual value of what we are parsing
            if (isLast) {
                temp[name] = arrayItem.value;
            }

            lastObject[index] = temp;
            setGetter(name, temp)
            return temp[name];
        }

        //this is the end of the names, this value will be the actual value of what we are parsing
        if (isLast) {
            if (!lastObject[name]) {
                lastObject[name] = arrayItem.value;
            }
            
            setGetter(name, lastObject)
            return lastObject[name]
        }

        //current name is an array
        if (isArray(name)) {
            var newName = getArrayName(name)
            if (!lastObject[newName]) {
                lastObject[newName] = []
            }

            setGetter(newName, lastObject)

            return lastObject[newName];
        }

        if (!lastObject[name]) {
            lastObject[name] = {}
        }

        setGetter(name, lastObject)

        return lastObject[name]
    }

    $.each(array, function () {
        var arrayItem = this;

        var names = arrayItem.name.split('.')

        var lastObject = result;

        $.each(names, function (i, name) {
            var lastName = names[i - 1]
            lastObject = loop(i, name, lastName, lastObject, arrayItem, names)
        });

    });

    return result;
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
        //ensure getter exists
        model["get_" + property] = function () {
            return this[property];
        }

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