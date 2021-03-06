﻿function registerNamespace(ns) {
    var nsParts = ns.split(".");
    var root = this;

    for (var i = 0; i < nsParts.length; i++) {
        if (typeof root[nsParts[i]] == "undefined")
            root[nsParts[i]] = {};

        root = root[nsParts[i]];
    }
}

registerNamespace('System')

registerNamespace('Microsoft.FSharp.Core')

registerNamespace('Microsoft.FSharp.Collections')

Microsoft.FSharp.Core.Operators = {
    op_Addition: function (x) {
        return function (y) {
            if (x.op_Addition) {
                return x.op_Addition(y)
            }
            return y + x;
        }
    },

    op_Subtraction: function (x) {
        return function (y) {
            if (x.op_Subtraction) {
                return x.op_Subtraction(y)
            }
            return y - x;
        }
    },

    op_Multiply : function(x){
        return function(y){
            if(x.op_Multiply){
                return x.op_Multiply(y);
            }

            return y * x;
        }
    },

    op_Division: function (x) {
        return function (y) {
            if (x.op_Division) {
                return x.op_Division(y);
            }

            return y / x;
        }
    },

    op_LessThanOrEqual: function (x) {
        return function (y) {
            if (x.op_LessThanOrEqual) {
                return x.op_LessThanOrEqual(y)
            }

            return y <= x;
        }
    },

    op_LessThan: function (x) {
        return function (y) {
            if (x.op_LessThan) {
                return x.op_LessThan(y)
            }

            return y < x;
        }
    },

    op_GreaterThan: function (x) {
        return function (y) {
            if (x.op_GreaterThan) {
                return x.op_GreaterThan(y)
            }

            return y > x;
        }
    },

    op_GreaterThanOrEqual: function (x) {
        return function (y) {
            if (x.op_GreaterThanOrEqual) {
                return x.op_GreaterThanOrEqual(y)
            }
            return y >= x;
        }
    },

    op_Inequality: function (x) {
        return function (y) {
            return Microsoft.FSharp.Core.Operators.op_Equality(y)(x) == false;
        }
    },

    op_Equality: function (one) {
        return function (two) {
            if (one == null && two == null)
                return true;

            if (one == null && two != null)
                return false;

            if (one != null && one.Equality) {
                return one.Equality(two);
            };

            return one === two;
        };
    },

    ToDouble: function (x) {
        return x;
    },

    op_Append: function (item1) {
        return function (item2) {
            var list = item1;
            var list2 = Microsoft.FSharp.Collections.ListModule.Reverse(item2)
            while (list2.read()) {
                var temp = list2.get();
                list = new Microsoft.FSharp.Collections.FSharpList.Cons(list, temp);
            }

            return list;
        }
    },

    FailWith: function (msg) {
        throw msg
    },

    FailurePattern: function (msg) {
        if (msg != null) {
            return new Microsoft.FSharp.Core.FSharpOption.Some(msg)
        }
        else {
            return new Microsoft.FSharp.Core.FSharpOption.None()
        }
    },

    op_PipeRight: function (func) {
        return function (item) {
            return func(item)
        }
    },

    Ignore: function (value) {
        return null
    },

    Fst: function (tup) { return tup.Item1; },
    Snd: function (tup) { return tup.Item2; },

    op_Range: function (end) {
        return function (start) {
            return new Range(start, end)
        }
    },

    CreateSequence: function (source) {
        return new Sequence(source)
    },

    ToInt: function (x) {
        if (x instanceof System.Enum)
            return x.Integer;

        return x;
    },

    ToString: function (x) {
        if (x instanceof System.Enum)
            return x.Text;

        return x;
    },

    Reference: function (x) {
        this.Value = x;
        this.get_Value = function () {
            return this.Value;
        }
    },

    Ref: function (x) {
        return new Microsoft.FSharp.Core.Operators.Reference(x);
    },

    op_ColonEquals: function (x) {
        return function (item) {
            item.Value = x;
        }
    },

    op_Dereference: function (x) {
        return x.Value;
    }


};

function Raise(exception) {
    throw exception;
}


function MatchFailureException(file, line, character) {
    return { file: file, line: line, character: character };
}

function Tuple() {
    for (var i = 1; i <= arguments.length + 1; i++) {
        this['Item' + i] = arguments[i - 1];
    }
}

Microsoft.FSharp.Core.FSharpOption = {
    None: function () {
        this.IsNone = true;
        this.IsSome = false;
        this.Value = null;

        this.get_Value = function () {
            return this.Value;
        };
    },

    Some: function (val) {
        this.IsNone = false;
        this.IsSome = true;
        this.Value = val;

        this.get_Value = function () {
            return this.Value;
        };
    },

    get_IsSome: function (x) {
        return x instanceof Microsoft.FSharp.Core.FSharpOption.Some
    },

    get_IsNone: function (x) {
        return x instanceof Microsoft.FSharp.Core.FSharpOption.None
    }
}


function Range(start, end) {
    this.start = start
    this.end = end
    this.current = start - 1
}

Range.prototype.get = function () {

    if (this.current <= this.end)
        return this.current
    else
        return null
}
Range.prototype.read = function () {
    this.current++
    if (this.current <= this.end)
        return true
    else
        return false
}

function Map(func, source) {
    this.func = func
    this.source = source
}

Map.prototype.get = function () {
    return this.func(this.source.get())
}

Map.prototype.read = function () {
    return this.source.read()
}

function Filter(func, source) {
    this.source = source
    this.func = func
}

Filter.prototype.get = function () {
    return this.source.get()
}

Filter.prototype.read = function () {
    if (!this.source.read())
        return false

    if (this.func(this.source.get()))
        return true

    return this.read()
}

Microsoft.FSharp.Collections.SeqModule = {
    Iterate: function (source) {
        return function (func) {
            while (source.read()) {
                func(source.get())
            }
        }
    },

    Delay: function (func) {
        return func();
    },

    Map: function (a) {
        return function (b) {
            return new Map(b, a)
        }
    },

    Filter: function (a) {
        return function (b) {
            return new Filter(b, a)
        }
    },

    ToArray: function (source) {
        var arr = []
        while (source.read())
            arr.push(source.get())
        return arr
    },

    Collect: function (a) {
        return function (b) {
            return new Concat(new Map(b, a))
        }
    },

    ToList: function (sequence) {
        var list = new Microsoft.FSharp.Collections.FSharpList.Empty();
        while (sequence.read()) {
            var temp = sequence.get();
            list = new Microsoft.FSharp.Collections.FSharpList.Cons(list, temp);
        }

        return list;
    },

    Exists: function (seq) {
        return function (func) {
            var result = false;

            while (seq.read()) {
                var item = seq.get()

                var temp = func(item)
                if (temp)
                    result = true;
            }

            return result;
        }
    }

}


function Sequence(source) {
    this.source = source
}

Sequence.prototype.get = function () {
    return this.source.get()
}

Sequence.prototype.read = function () {
    return this.source.read()
}



Sequence.prototype.ToArray = function () {
    return Microsoft.FSharp.Collections.SeqModule.ToArray(this)
}



function Concat(sources) {
    this.sources = sources
    this.currentSource = null
}

Concat.prototype.read = function () {
    if (this.currentSource == null) {
        this.sources.read();
        this.currentSource = this.sources.get()
    }

    var current = this.currentSource.read()
    if (current)
        return true;

    var hasMoreSources = this.sources.read();
    if (!hasMoreSources)
        return false


    this.currentSource = this.sources.get()

    return this.currentSource.read()
}

Concat.prototype.get = function () {
    return this.currentSource.get()
}

Array.prototype.read = function () {
    if (this.position == null) {
        if (this.length == 0)
            return false

        this.position = -1

    }
    var temp = this.position < (this.length - 1)
    if (temp) {
        this.position++
        return true
    }
    else {
        this.position = null
        return false
    }
}
Array.prototype.get = function () {
    return this[this.position]
}

Array.prototype.get_Length = function () {
    return this.Length;
}


registerNamespace('Microsoft.FSharp.Collections');

Microsoft.FSharp.Collections.ArrayModule = {
    Fold: function (source) {
        return function (acc) {
            return function (func) {
                //list = ListModule.Reverse(list)
                while (source.read()) {
                    var next = source.get()
                    acc = func(acc)(next)
                }

                return acc
            }
        }
    },

    Map: function (source) {
        return function (func) {
            var newArr = []
            while (source.read()) {
                var item = source.get()
                var result = func(item)
                newArr.push(result)
            }

            return newArr
        }
    },

    Iterate: function (source) {
        return function (func) {
            while (source.read()) {
                var item = source.get()
                func(item)
            }
        }
    },

    Filter: function (source) {
        return function (func) {
            var newArr = []
            while (source.read()) {
                var item = source.get()
                if (func(item)) {
                    newArr.push(item)
                }
            }

            return newArr;
        }
    }
}

Microsoft.FSharp.Collections.FSharpList = {
    Empty : function () {
        this.Length = 0
        this.Head = null
        this.IsEmpty = true
        this.Tail = null
        this.get_Item = function (x) {
            return null
        };

        this.get_Length = function () {
            return this.Length
        };

        this.get_Head = function () {
            return this.Head
        };

        this.get_IsEmpty = function () {
            return this.IsEmpty
        };

        this.get_Tail = function () {
            return this.Tail
        };

        this.read = function () {
            return false
        };

        this.get = function(){
            return null;
        };
    },

    Cons : function (list, arg) {
        this.ReadState = null;
        this.Length = list.Length + 1;
        this.Head = arg;
        this.IsEmpty = false;
        this.Tail = list;
        
        this.get_Item = function (x) {
            if (x == 0)
                return this.Head;
            else
                return this.Tail.get_Item(x - 1);
        };

        this.get_Length = function () {
            return this.Length
        };

        this.get_Head = function () {
            return this.Head
        };

        this.get_IsEmpty = function () {
            return this.IsEmpty
        };

        this.get_Tail = function () {
            return this.Tail
        };


        this.read = function () {
            if (this.ReadState == null)
                this.ReadState = this.Length

            this.ReadState--;
            if (this.ReadState < 0) {
                this.ReadState = null
                return false;
            }


            return true;
        };

        this.get = function () {
            return this.get_Item(this.ReadState)
        };

    }
}

Microsoft.FSharp.Collections.ListModule = {
    Reverse: function (sequence) {
        var array = [];
        while (sequence.read()) {
            var temp = sequence.get();
            array.push(temp);
        }

        var list = new Microsoft.FSharp.Collections.FSharpList.Empty();
        for (var i = array.length - 1; i >= 0; i--) {
            list = new Microsoft.FSharp.Collections.FSharpList.Cons(list, array[i])
        }

        return list;
    },

    Exists: function (list) {
        return function (func) {
            var result = false;
            while (list.read()) {
                var item = list.get()

                var temp = func(item)
                if (temp == true)
                    result = true;
            }

            return result;
        }
    },

    Fold: function (list) {
        return function (acc) {
            return function (func) {
                list = Microsoft.FSharp.Collections.ListModule.Reverse(list);
                while (list.read()) {
                    var next = list.get();
                    acc = func(acc)(next);
                }

                return acc;
            }
        }
    }
}



registerNamespace('Microsoft.FSharp.Core.LanguagePrimitives')
Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicFunctions = {
    UnboxGeneric : function (x) { return x; }
}


Microsoft.FSharp.Collections.FSharpMap = {
    Empty : function () {
        this.Count = 0
        this.Head = null
        this.IsEmpty = true
        this.Tail = null
        this.get_Item = function (x) {
            return null
        };

        this.get_Count = function () {
            return this.Count
        };

        this.get_Head = function () {
            return this.Head
        };

        this.get_IsEmpty = function () {
            return this.IsEmpty
        };

        this.get_Tail = function () {
            return this.Tail
        };

        this.read = function () {
            return false
        };

        this.ContainsKey = function (key) {
            return false;
        };

        this.Add = function (value) {
            var self = this
            return function (key) {
                return Microsoft.FSharp.Collections.MapModule.Add(self)(value)(key);
            }
        };

        this.Remove = function (key) {
            var result = new Microsoft.FSharp.Collections.FSharpMap.Empty()
            while (this.read()) {
                var item = this.get()
                if (item.key != key) {
                    result = new Microsoft.FSharp.Collections.FSharpMap.Cons(result, item)
                }
            }

            return result;
        };

    },

    Cons : function (list, arg) {
        this.ReadState = null;
        this.Count = list.Count + 1;
        this.Head = arg;
        this.IsEmpty = false;
        this.Tail = list;

        this.get_Item = function (x) {
            if (x == 0)
                return this.Head;
            else
                return this.Tail.get_Item(x - 1);
        };

        this.get_Count = function () {
            return this.Count
        };

        this.get_Head = function () {
            return this.Head
        };

        this.get_IsEmpty = function () {
            return this.IsEmpty
        };

        this.get_Tail = function () {
            return this.Tail
        };

        this.Add = function (value) {
            var self = this
            return function (key) {
                return Microsoft.FSharp.Collections.MapModule.Add(self)(value)(key);
            }
        };

        this.Remove = function (key) {
            var result = new Microsoft.FSharp.Collections.FSharpMap.Empty()
            while (this.read()) {
                var item = this.get()
                if (item.key != key) {
                    result = new Microsoft.FSharp.Collections.FSharpMap.Cons(result, item)
                }
            }

            return result;
        };

        this.read = function () {
            if (this.ReadState == null)
                this.ReadState = this.Count

            this.ReadState--;
            if (this.ReadState < 0) {
                this.ReadState = null
                return false;
            }


            return true;
        };

        this.get = function () {
            return this.get_Item(this.ReadState)
        };

        this.ContainsKey = ContainsKey = function (key) {
            return Microsoft.FSharp.Collections.MapModule.ContainsKey(this)(key)
        }
    }

}

Microsoft.FSharp.Collections.MapModule = {
    Empty : function () {
        return new Microsoft.FSharp.Collections.FSharpMap.Empty();
    },
    Add : function (source) {
        return function (value) {
            return function (key) {
                var item = { key: key, value: value }
                return new Microsoft.FSharp.Collections.FSharpMap.Cons(source, item)
            }
        }
    },

    Find : function (source) {
        return function (key) {
            var result = null;
            while (source.read()) {
                var item = source.get()
                if (item.key == key) {
                    result = item.value;
                }
            }

            return result;
        }
    },

    TryFind : function (source) {
        return function (key) {
            var result = new Microsoft.FSharp.Core.FSharpOption.None();
            while (source.read()) {
                var item = source.get()
                if (item.key == key) {
                    result = new Microsoft.FSharp.Core.FSharpOption.Some(item.value);
                }
            }

            return result
        }
    },

    ContainsKey : function (source) {
        return function (key) {
            var result = false;
            while (source.read()) {
                var item = source.get()
                if (item.key == key)
                    result = true;
            }

            return result;
        }
    }
}

Pad = function (number, length) {
    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }

    return str;
}

System.Enum = function () { }
System.Enum.prototype.toString = function () {
    return this.Text;
}

System.Enum.prototype.Equality = function (x) {
    return x.Integer == this.Integer;
}



System.DateTime = function () {
    this.Year = 0001
    this.Month = 1
    this.Day = 1
    this.Hour = 12
    this.Minute = 0
    this.Second = 0

    if (arguments.length == 3) {
        this.Year = arguments[0]
        this.Month = arguments[1]
        this.Day = arguments[2]
    }

    if (arguments.length == 6) {
        this.Year = arguments[0]
        this.Month = arguments[1]
        this.Day = arguments[2]
        this.Hour = arguments[3]
        this.Minute = arguments[4]
        this.Second = arguments[5]
    }


    this.toString = function () {

        var amPm = this.Hour > 12 ? "pm" : "am"
        return this.Month + "/" + this.Day + "/" + Pad(this.Year, 4) + " " + this.Hour + ":" + Pad(this.Minute, 2) + ":" + Pad(this.Second, 2) + " " + amPm;
    };

    this.ToShortDateString = function () {
        var amPm = this.Hour > 12 ? "pm" : "am"
        return this.Month + "/" + this.Day + "/" + Pad(this.Year, 4);
    };

    this.AddYears = function (x) {
        return new System.DateTime(this.Year + x, this.Month, this.Day, this.Hour, this.Minute, this.Second)
    };

    this.AddMonths = function (x) {
        var d = new Date(this.Year, ((this.Month - 1) + x), this.Day)

        return new System.DateTime(d.getFullYear(), d.getMonth() + 1, d.getDate(), this.Hour, this.Minute, this.Second)
    };

    this.AddDays = function (x) {
        var d = new Date(this.Year, (this.Month - 1), this.Day + x)

        return new System.DateTime(d.getFullYear(), d.getMonth() + 1, d.getDate(), this.Hour, this.Minute, this.Second)
    };


    this.Equality = function (x) {
        var result = true;
        result = result && this.Year === x.Year;
        result = result && this.Month === x.Month;
        result = result && this.Day === x.Day;
        result = result && this.Hour === x.Hour;
        result = result && this.Minute === x.Minute;
        result = result && this.Second === x.Second;

        return result;
    };

    var getJavascriptDate = function (x) {
        var date = new Date(x.Year, (x.Month - 1), x.Day, x.Hour, x.Minute, x.Second)

        return date;
    }

    this.op_GreaterThan = function (x) {
        var date1 = getJavascriptDate(this)
        var date2 = getJavascriptDate(x)

        return date2 > date1;
    }

    this.op_LessThan = function (x) {
        var date1 = getJavascriptDate(this)
        var date2 = getJavascriptDate(x)

        return date2 < date1;
    }

    this.op_GreaterThanOrEqual = function (x) {
        var date1 = getJavascriptDate(this)
        var date2 = getJavascriptDate(x)

        return date2 >= date1;
    }

    this.op_LessThanOrEqual = function (x) {
        var date1 = getJavascriptDate(this)
        var date2 = getJavascriptDate(x)

        return date2 <= date1;
    }


}

System.DateTime.get_Now = function () {
    var d = new Date()
    return new System.DateTime(d.getFullYear(), d.getMonth() + 1, d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds())
};

System.DateTime.Parse = function (x) {
    var d = new Date(x)
    var hours = d.getHours() > 0 ? d.getHours() : 12;

    return new System.DateTime(d.getFullYear(), d.getMonth() + 1, d.getDate(), hours, d.getMinutes(), d.getSeconds())
};

System.DateTime.MinValue = new System.DateTime();

String.prototype.Contains = function (x) {
    return this.indexOf(x) != -1
}