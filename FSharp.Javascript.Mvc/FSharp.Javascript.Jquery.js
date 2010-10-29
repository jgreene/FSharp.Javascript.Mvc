registerNamespace('FSharp.Javascript')

FSharp.Javascript.Jquery = {}
FSharp.Javascript.Jquery.jquery = window.jQuery

jQuery.fn.value = jQuery.fn.val

$.extend($.fn, {
    fsharpBind: function (eventName) {
        var self = this;
        return function (func) {
            self.bind(eventName, func)
        }
    }
})