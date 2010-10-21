registerNamespace("FormValidator");
FormValidator.FormValidator = function (form, prefix, validators) {
    this.Validators = validators;
    this.Prefix = prefix;
    this.Form = form;
};
FormValidator.FormValidator.prototype.Equality = function (compareTo) {
    var result = true;
    result = ((result) && (Microsoft.FSharp.Core.Operators.op_Equality(this.get_Form)(compareTo.get_Form)));
    result = ((result) && (Microsoft.FSharp.Core.Operators.op_Equality(this.get_Prefix)(compareTo.get_Prefix)));
    result = ((result) && (Microsoft.FSharp.Core.Operators.op_Equality(this.get_Validators)(compareTo.get_Validators)));
    return result;
};
FormValidator.FormValidator.prototype.get_Form = function () {
    return this.Form
};
FormValidator.FormValidator.prototype.get_Prefix = function () {
    return this.Prefix
};
FormValidator.FormValidator.prototype.get_Validators = function () {
    return this.Validators
};
FormValidator.FormValidator.prototype.set_Form = function (x) {
    this.Form = x
};
FormValidator.FormValidator.prototype.set_Prefix = function (x) {
    this.Prefix = x
};
FormValidator.FormValidator.prototype.set_Validators = function (x) {
    this.Validators = x
};
FormValidator.Validator = function (errorField, fieldNames, validator) {
    this.Validator = validator;
    this.FieldNames = fieldNames;
    this.ErrorField = errorField;
};
FormValidator.Validator.prototype.Equality = function (compareTo) {
    var result = true;
    result = ((result) && (Microsoft.FSharp.Core.Operators.op_Equality(this.get_ErrorField)(compareTo.get_ErrorField)));
    result = ((result) && (Microsoft.FSharp.Core.Operators.op_Equality(this.get_FieldNames)(compareTo.get_FieldNames)));
    result = ((result) && (Microsoft.FSharp.Core.Operators.op_Equality(this.get_Validator)(compareTo.get_Validator)));
    return result;
};
FormValidator.Validator.prototype.get_ErrorField = function () {
    return this.ErrorField
};
FormValidator.Validator.prototype.get_FieldNames = function () {
    return this.FieldNames
};
FormValidator.Validator.prototype.get_Validator = function () {
    return this.Validator
};
FormValidator.Validator.prototype.set_ErrorField = function (x) {
    this.ErrorField = x
};
FormValidator.Validator.prototype.set_FieldNames = function (x) {
    this.FieldNames = x
};
FormValidator.Validator.prototype.set_Validator = function (x) {
    this.Validator = x
};
FormValidator.setupValidation = function (formValidator) {
    var form = new FSharp.Javascript.Jquery.jquery(Microsoft.FSharp.Core.Operators.op_Addition(formValidator.get_Form())("#"));
    var getInputName = function (fieldName) {
        return (function () {
            if (Microsoft.FSharp.Core.Operators.op_Equality("")(formValidator.get_Prefix())) {
                return Microsoft.FSharp.Core.Operators.op_Addition("']")(Microsoft.FSharp.Core.Operators.op_Addition(fieldName)("input[name='"))
            } else {
                return Microsoft.FSharp.Core.Operators.op_Addition("']")(Microsoft.FSharp.Core.Operators.op_Addition(fieldName)(Microsoft.FSharp.Core.Operators.op_Addition(".")(Microsoft.FSharp.Core.Operators.op_Addition(formValidator.get_Prefix())("input[name='"))))
            };
        })()
    };
    var getElement = function (prop) {
        return (function () {
            if (Microsoft.FSharp.Core.Operators.op_Equality("")(formValidator.get_Prefix())) {
                return new FSharp.Javascript.Jquery.jquery(Microsoft.FSharp.Core.Operators.op_Addition(prop)("#"))
            } else {
                return new FSharp.Javascript.Jquery.jquery(Microsoft.FSharp.Core.Operators.op_Addition(prop)(Microsoft.FSharp.Core.Operators.op_Addition(".")(Microsoft.FSharp.Core.Operators.op_Addition(formValidator.get_Prefix())("#"))))
            };
        })()
    };
    var getErrorElement = function (elem) {
        var id = Microsoft.FSharp.Core.Operators.op_Addition("_validationMessage")(elem.attr("id"));
        return new FSharp.Javascript.Jquery.jquery(Microsoft.FSharp.Core.Operators.op_Addition(id)("#"));
    };
    var errors = Microsoft.FSharp.Core.Operators.Ref(Microsoft.FSharp.Collections.MapModule.Empty());
    var addError = function (property) {
        return function (errorMessage) {
            return (function () {
                if (errors.get_Value().ContainsKey(property)) {
                    return (function () {
                        var propertyErrors = Microsoft.FSharp.Core.Operators.op_PipeRight(function (table) {
                            return Microsoft.FSharp.Collections.MapModule.TryFind(table)(property)
                        })(errors.get_Value());
                        return (function () {
                            if (Microsoft.FSharp.Core.FSharpOption.get_IsSome(propertyErrors)) {
                                return (function () {
                                    if (Microsoft.FSharp.Core.Operators.op_PipeRight(function (list) {
                                        return Microsoft.FSharp.Collections.ListModule.Exists(list)(function (x) {
                                            return Microsoft.FSharp.Core.Operators.op_Equality(errorMessage)(x)
                                        })
                                    })(propertyErrors.get_Value())) {
                                        return null
                                    } else {
                                        return Microsoft.FSharp.Core.Operators.op_ColonEquals(errors.get_Value().Remove(property).Add(new Microsoft.FSharp.Collections.FSharpList.Cons(propertyErrors.get_Value(), errorMessage))(property))(errors)
                                    };
                                })()
                            } else {
                                return null
                            };
                        })();
                    })()
                } else {
                    return Microsoft.FSharp.Core.Operators.op_ColonEquals(errors.get_Value().Add(new Microsoft.FSharp.Collections.FSharpList.Cons(new Microsoft.FSharp.Collections.FSharpList.Empty(), errorMessage))(property))(errors)
                };
            })()
        }
    };
    var getErrors = function (property) {
        return (function () {
            if (errors.get_Value().ContainsKey(property)) {
                return Microsoft.FSharp.Core.Operators.op_PipeRight(function (table) {
                    return Microsoft.FSharp.Collections.MapModule.Find(table)(property)
                })(errors.get_Value())
            } else {
                return new Microsoft.FSharp.Collections.FSharpList.Empty()
            };
        })()
    };
    var displayErrors = function (property) {
        var elem = getElement(property);
        var errorElement = getErrorElement(elem);
        return (function () {
            if (errors.get_Value().ContainsKey(property)) {
                return (function () {
                    var errs = Microsoft.FSharp.Core.Operators.op_PipeRight(function (table) {
                        return Microsoft.FSharp.Collections.MapModule.TryFind(table)(property)
                    })(errors.get_Value());
                    return (function () {
                        if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(errs)) {
                            return Microsoft.FSharp.Core.Operators.op_PipeRight(function (value) {
                                return Microsoft.FSharp.Core.Operators.Ignore(value)
                            })(errorElement.hide())
                        } else {
                            return (function () {
                                var errorMessage = Microsoft.FSharp.Core.Operators.op_PipeRight(function (list) {
                                    return Microsoft.FSharp.Collections.ListModule.Fold(list)("")(function (acc) {
                                        return function (next) {
                                            return Microsoft.FSharp.Core.Operators.op_Addition(acc)(Microsoft.FSharp.Core.Operators.op_Addition("<br/>")(next))
                                        }
                                    })
                                })(errs.get_Value());
                                return Microsoft.FSharp.Core.Operators.op_PipeRight(function (value) {
                                    return Microsoft.FSharp.Core.Operators.Ignore(value)
                                })(errorElement.html(errorMessage).show());
                            })()
                        };
                    })();
                })()
            } else {
                return Microsoft.FSharp.Core.Operators.op_PipeRight(function (value) {
                    return Microsoft.FSharp.Core.Operators.Ignore(value)
                })(errorElement.hide())
            };
        })();
    };
    var resetErrors = function (property) {
        return (function () {
            if (errors.get_Value().ContainsKey(property)) {
                return (function () {
                    Microsoft.FSharp.Core.Operators.op_ColonEquals(errors.get_Value().Remove(property))(errors);
                    return displayErrors(property);
                })()
            } else {
                return null
            };
        })()
    };
    var checkTypes = function (props) {
        return function (model) {
            var getValue = function (property) {
                return FormValidator.getValueFromModel(property)(model)
            };
            var setValue = function (tupledArg) {
                var prop = tupledArg.Item1;
                var value = tupledArg.Item2;
                return FormValidator.setValueOnModel(value)(prop)(model);
            };
            var result = Microsoft.FSharp.Core.Operators.Ref(true);
            var error = function (prop) {
                return function (errorMessage) {
                    addError(prop)(errorMessage);
                    displayErrors(prop);
                    return Microsoft.FSharp.Core.Operators.op_ColonEquals(false)(result);
                }
            };
            Microsoft.FSharp.Core.Operators.op_PipeRight(function (array) {
                return Microsoft.FSharp.Collections.ArrayModule.Iterate(array)(function (tupledArg) {
                    var prop = tupledArg.Item1;
                    var typ = tupledArg.Item2;
                    var value = getValue(prop);
                    return (function () {
                        if (Microsoft.FSharp.Core.Operators.op_Equality("DateTime option")(typ)) {
                            return (function () {
                                var parsed = FSharp.Javascript.Library.DateTime.TryParse2.Static(value);
                                return (function () {
                                    if ((function () {
                                        if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                            return Microsoft.FSharp.Core.Operators.op_Inequality("")(value)
                                        } else {
                                            return false
                                        };
                                    })()) {
                                        return error(prop)("Invalid DateTime option")
                                    } else {
                                        return (function (tupledArg) {
                                            var arg00 = tupledArg.Item1;
                                            var arg01 = tupledArg.Item2;
                                            return setValue(new Tuple(arg00, arg01));
                                        })(new Tuple(prop, parsed))
                                    };
                                })();
                            })()
                        } else {
                            return (function () {
                                if (Microsoft.FSharp.Core.Operators.op_Equality("Boolean option")(typ)) {
                                    return (function () {
                                        var parsed = FSharp.Javascript.Library.Boolean.TryParse2.Static(value);
                                        return (function () {
                                            if ((function () {
                                                if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                                    return Microsoft.FSharp.Core.Operators.op_Inequality("")(value)
                                                } else {
                                                    return false
                                                };
                                            })()) {
                                                return error(prop)("Invalid Boolean option")
                                            } else {
                                                return (function (tupledArg) {
                                                    var arg00 = tupledArg.Item1;
                                                    var arg01 = tupledArg.Item2;
                                                    return setValue(new Tuple(arg00, arg01));
                                                })(new Tuple(prop, parsed))
                                            };
                                        })();
                                    })()
                                } else {
                                    return (function () {
                                        var x = typ;
                                        return (function () {
                                            if ((function () {
                                                if (x.Contains("Int")) {
                                                    return x.Contains("option")
                                                } else {
                                                    return false
                                                };
                                            })()) {
                                                return (function () {
                                                    var parsed = FSharp.Javascript.Library.Int16.TryParse2.Static(value);
                                                    return (function () {
                                                        if ((function () {
                                                            if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                                                return Microsoft.FSharp.Core.Operators.op_Inequality("")(value)
                                                            } else {
                                                                return false
                                                            };
                                                        })()) {
                                                            return error(prop)("Invalid Integer option")
                                                        } else {
                                                            return (function (tupledArg) {
                                                                var arg00 = tupledArg.Item1;
                                                                var arg01 = tupledArg.Item2;
                                                                return setValue(new Tuple(arg00, arg01));
                                                            })(new Tuple(prop, parsed))
                                                        };
                                                    })();
                                                })()
                                            } else {
                                                return (function () {
                                                    if (Microsoft.FSharp.Core.Operators.op_Equality("DateTime")(typ)) {
                                                        return (function () {
                                                            var parsed = FSharp.Javascript.Library.DateTime.TryParse2.Static(value);
                                                            return (function () {
                                                                if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                                                    return error(prop)("Invalid DateTime")
                                                                } else {
                                                                    return null
                                                                };
                                                            })();
                                                        })()
                                                    } else {
                                                        return (function () {
                                                            var x = typ;
                                                            return (function () {
                                                                if (x.Contains("Int")) {
                                                                    return (function () {
                                                                        var parsed = FSharp.Javascript.Library.Int16.TryParse2.Static(value);
                                                                        return (function () {
                                                                            if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                                                                return error(prop)("Invalid Integer")
                                                                            } else {
                                                                                return (function (tupledArg) {
                                                                                    var arg00 = tupledArg.Item1;
                                                                                    var arg01 = tupledArg.Item2;
                                                                                    return setValue(new Tuple(arg00, arg01));
                                                                                })(new Tuple(prop, parsed.get_Value()))
                                                                            };
                                                                        })();
                                                                    })()
                                                                } else {
                                                                    return (function () {
                                                                        if (Microsoft.FSharp.Core.Operators.op_Equality("Decimal")(typ)) {
                                                                            return (function () {
                                                                                var parsed = FSharp.Javascript.Library.Decimal.TryParse2.Static(value);
                                                                                return (function () {
                                                                                    if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                                                                        return error(prop)("Invalid Decimal")
                                                                                    } else {
                                                                                        return (function (tupledArg) {
                                                                                            var arg00 = tupledArg.Item1;
                                                                                            var arg01 = tupledArg.Item2;
                                                                                            return setValue(new Tuple(arg00, arg01));
                                                                                        })(new Tuple(prop, parsed.get_Value()))
                                                                                    };
                                                                                })();
                                                                            })()
                                                                        } else {
                                                                            return (function () {
                                                                                if (Microsoft.FSharp.Core.Operators.op_Equality("Double")(typ)) {
                                                                                    return (function () {
                                                                                        var parsed = FSharp.Javascript.Library.Decimal.TryParse2.Static(value);
                                                                                        return (function () {
                                                                                            if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                                                                                return error(prop)("Invalid Decimal")
                                                                                            } else {
                                                                                                return (function (tupledArg) {
                                                                                                    var arg00 = tupledArg.Item1;
                                                                                                    var arg01 = tupledArg.Item2;
                                                                                                    return setValue(new Tuple(arg00, arg01));
                                                                                                })(new Tuple(prop, parsed.get_Value()))
                                                                                            };
                                                                                        })();
                                                                                    })()
                                                                                } else {
                                                                                    return (function () {
                                                                                        if (Microsoft.FSharp.Core.Operators.op_Equality("Single")(typ)) {
                                                                                            return (function () {
                                                                                                var parsed = FSharp.Javascript.Library.Decimal.TryParse2.Static(value);
                                                                                                return (function () {
                                                                                                    if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                                                                                        return error(prop)("Invalid Decimal")
                                                                                                    } else {
                                                                                                        return (function (tupledArg) {
                                                                                                            var arg00 = tupledArg.Item1;
                                                                                                            var arg01 = tupledArg.Item2;
                                                                                                            return setValue(new Tuple(arg00, arg01));
                                                                                                        })(new Tuple(prop, parsed.get_Value()))
                                                                                                    };
                                                                                                })();
                                                                                            })()
                                                                                        } else {
                                                                                            return (function () {
                                                                                                if (Microsoft.FSharp.Core.Operators.op_Equality("Boolean")(typ)) {
                                                                                                    return (function () {
                                                                                                        var parsed = FSharp.Javascript.Library.Boolean.TryParse2.Static(value);
                                                                                                        return (function () {
                                                                                                            if (Microsoft.FSharp.Core.FSharpOption.get_IsNone(parsed)) {
                                                                                                                return error(prop)("Invalid Boolean")
                                                                                                            } else {
                                                                                                                return (function (tupledArg) {
                                                                                                                    var arg00 = tupledArg.Item1;
                                                                                                                    var arg01 = tupledArg.Item2;
                                                                                                                    return setValue(new Tuple(arg00, arg01));
                                                                                                                })(new Tuple(prop, parsed.get_Value()))
                                                                                                            };
                                                                                                        })();
                                                                                                    })()
                                                                                                } else {
                                                                                                    return null
                                                                                                };
                                                                                            })()
                                                                                        };
                                                                                    })()
                                                                                };
                                                                            })()
                                                                        };
                                                                    })()
                                                                };
                                                            })();
                                                        })()
                                                    };
                                                })()
                                            };
                                        })();
                                    })()
                                };
                            })()
                        };
                    })();
                })
            })(props);
            return result.get_Value();
        }
    };
    Microsoft.FSharp.Core.Operators.op_PipeRight(function (source) {
        return Microsoft.FSharp.Collections.SeqModule.Iterate(source)(function (validator) {
            var field = validator.get_ErrorField();
            var properties = validator.get_FieldNames();
            var inputName = getInputName(field);
            var input = form.find(inputName);
            Microsoft.FSharp.Core.Operators.op_PipeRight(function (value) {
                return Microsoft.FSharp.Core.Operators.Ignore(value)
            })(input.fsharpBind(function (unitVar0) {
                var model = FormValidator.getFormModel(formValidator);
                return (function () {
                    if (checkTypes(properties)(model)) {
                        return (function () {
                            var result = validator.get_Validator()(model);
                            return (function () {
                                if (result instanceof Microsoft.FSharp.Core.FSharpOption.None) {
                                    return true
                                } else {
                                    return (function () {
                                        var x = result.get_Value();
                                        addError(field)(x);
                                        return false;
                                    })()
                                };
                            })();
                        })()
                    } else {
                        return false
                    };
                })();
            })("FSharpValidate"));
            return Microsoft.FSharp.Core.Operators.op_PipeRight(function (value) {
                return Microsoft.FSharp.Core.Operators.Ignore(value)
            })(input.blur(function (unitVar0) {
                resetErrors(field);
                Microsoft.FSharp.Core.Operators.op_PipeRight(function (value) {
                    return Microsoft.FSharp.Core.Operators.Ignore(value)
                })(input.triggerHandler("FSharpValidate"));
                return displayErrors(field);
            }));
        })
    })(formValidator.get_Validators());
    return Microsoft.FSharp.Core.Operators.op_PipeRight(function (value) {
        return Microsoft.FSharp.Core.Operators.Ignore(value)
    })(form.submit(function (unitVar0) {
        Microsoft.FSharp.Core.Operators.op_PipeRight(function (source) {
            return Microsoft.FSharp.Collections.SeqModule.Iterate(source)(function (validator) {
                var field = validator.get_ErrorField();
                var inputName = getInputName(field);
                var input = form.find(inputName);
                resetErrors(field);
                Microsoft.FSharp.Core.Operators.op_PipeRight(function (value) {
                    return Microsoft.FSharp.Core.Operators.Ignore(value)
                })(input.triggerHandler("FSharpValidate"));
                return displayErrors(field);
            })
        })(formValidator.get_Validators());
        return Microsoft.FSharp.Core.Operators.op_Equality((0))(errors.get_Value().get_Count());
    }));
}; ; ;
if (FormValidator.main) {
    FormValidator.main()
};

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
                    if (lastObj == null) lastObj = obj;

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

    if (formValidator.Prefix == "") return model

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