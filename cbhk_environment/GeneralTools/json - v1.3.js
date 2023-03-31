function get__nodeValue(str_json, recursion) { for (var i in str_json) { var a = Object.prototype.toString.call(str_json[i]); if (a == "[object Object]" || a == "[object Array]") { if (recursion) { get__nodeValue(str_json[i], recursion) } } else if (a == "[object String]" || a == "[object Number]" || a == "[object Boolean]") { str_all.push(str_json[i]) } } return str_all } function get__count(c) { var obj = Object.prototype.toString.call(c); if (obj == '[object Array]') { return c.length } else { return 0 } } function get__name_All(a) { var obj = Object.prototype.toString.call(a); if (obj == '[object Object]') { var ary = new Array(); for (var key in a) { if (typeof a[key] !== 'function') { ary.push(key) } } return ary.join('\r\n') } else { return '' } } function get__extend(target, source) { if (Object.prototype.toString.call(target) == '[object Array]') { for (var i = 0; i < source.length; i++) { target.push(source[i]) } return target } else { var str_obj; for (var obj in source) { str_obj = typeof target[obj]; if (str_obj == 'string' || str_obj == 'boolean' || str_obj == 'undefined') { target[obj] = source[obj] } else if (str_obj == 'number') { target = target.concat(source) } else if (str_obj == 'object') { target[obj] = get__extend(target[obj], source[obj]) } } return target } } if (typeof JSON !== 'object') { JSON = {} } (function () { 'use strict'; function f(n) { return n < 10 ? '0' + n : n } if (typeof Date.prototype.toJSON !== 'function') { Date.prototype.toJSON = function (key) { return isFinite(this.valueOf()) ? this.getUTCFullYear() + '-' + f(this.getUTCMonth() + 1) + '-' + f(this.getUTCDate()) + 'T' + f(this.getUTCHours()) + ':' + f(this.getUTCMinutes()) + ':' + f(this.getUTCSeconds()) + 'Z' : null }; String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function (key) { return this.valueOf() } } var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, gap, indent, meta = { '\b': '\\b', '\t': '\\t', '\n': '\\n', '\f': '\\f', '\r': '\\r', '"': '\\"', '\\': '\\\\' }, rep; function quote(string) { escapable.lastIndex = 0; return escapable.test(string) ? '"' + string.replace(escapable, function (a) { var c = meta[a]; return typeof c === 'string' ? c : '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4) }) + '"' : '"' + string + '"' } function str(key, holder) { var i, k, v, length, mind = gap, partial, value = holder[key]; if (value && typeof value === 'object' && typeof value.toJSON === 'function') { value = value.toJSON(key) } if (typeof rep === 'function') { value = rep.call(holder, key, value) } switch (typeof value) { case 'string': return quote(value); case 'number': return isFinite(value) ? String(value) : 'null'; case 'boolean': case 'null': return String(value); case 'object': if (!value) { return 'null' } gap += indent; partial = []; if (Object.prototype.toString.apply(value) === '[object Array]') { length = value.length; for (i = 0; i < length; i += 1) { partial[i] = str(i, value) || 'null' } v = partial.length === 0 ? '[]' : gap ? '[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']' : '[' + partial.join(',') + ']'; gap = mind; return v } if (rep && typeof rep === 'object') { length = rep.length; for (i = 0; i < length; i += 1) { k = rep[i]; if (typeof k === 'string') { v = str(k, value); if (v) { partial.push(quote(k) + (gap ? ': ' : ':') + v) } } } } else { for (k in value) { if (Object.prototype.hasOwnProperty.call(value, k)) { v = str(k, value); if (v) { partial.push(quote(k) + (gap ? ': ' : ':') + v) } } } } v = partial.length === 0 ? '{}' : gap ? '{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}' : '{' + partial.join(',') + '}'; gap = mind; return v } } if (typeof JSON.stringify !== 'function') { JSON.stringify = function (value, replacer, space) { var i; gap = ''; indent = ''; if (typeof space === 'number') { for (i = 0; i < space; i += 1) { indent += ' ' } } else if (typeof space === 'string') { indent = space } rep = replacer; if (replacer && typeof replacer !== 'function' && (typeof replacer !== 'object' || typeof replacer.length !== 'number')) { throw new Error('JSON.stringify'); } return str('', { '': value }) } } if (typeof JSON.parse !== 'function') { JSON.parse = function (text, reviver) { var j; function walk(holder, key) { var k, v, value = holder[key]; if (value && typeof value === 'object') { for (k in value) { if (Object.prototype.hasOwnProperty.call(value, k)) { v = walk(value, k); if (v !== undefined) { value[k] = v } else { delete value[k] } } } } return reviver.call(holder, key, value) } text = String(text); cx.lastIndex = 0; if (cx.test(text)) { text = text.replace(cx, function (a) { return '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4) }) } if (/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@').replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) { j = eval('(' + text + ')'); return typeof reviver === 'function' ? walk({ '': j }, '') : j } throw new SyntaxError('JSON.parse'); } } if (!Object.prototype.toJSONString) { Object.prototype.toJSONString = function (filter) { return JSON.stringify(this, filter) }; Object.prototype.parseJSON = function (filter) { return JSON.parse(this, filter) } } }());
var data = null;
var obj_error = "";
var str_Code = String.fromCharCode(9216), str_reg = new RegExp(str_Code, 'g');
function parseJSON(text, isobject) {
    // 返回值为true / false，意思为解析是否成功。
    data = null;
    if (isobject == null || isobject == true) {
        try {
            data = text;
        } catch (error) {
            obj_error = error;
        }
    } else {
        try {
            data = JSON.parse(text);
        } catch (error) {
            obj_error = error;
            data = null;
        }
    }
    if (data == null) {
        return false;
    }
    return true;
}
function getError() {
    // 获取错误
    return obj_error;
}
function getRawText() {
    // 获取这个 JSON
    return JSON.stringify(data);
}
function getObjType(ObjName) {
    // 如果传入参数为空那么默认获取这个 JSON 本体的类型
    if (ObjName == null) {
        toTest = data;
    } else {
        toTest = data[ObjName];
    }
    var type = Object.prototype.toString.call (toTest);
    return type;
}
function getLength() {
    // 获取数组的大小
    return data.length;
}
function getAllNames() {
    // 获取所有子属性，如果为数组就会返回所有数组下标（没卵用）
    var arrs = [];
    for (var ele in data) {
        if (ele != 'parseJSON' && ele != 'toJSONString')
            arrs[arrs.length] = ele;
    }
    return arrs;
}

function resetJSON(isArray) {
    if (isArray == null || isArray == false)
        data = {};
    else
        data = [];
}

function hasObject(objName) {
    var obj = data[objName];
    return obj != null;
}
function getJSONObject(objName) {
    //获取子对象
    return data[objName];
}
function getRawObject(objName, isobject) {
    // 获取对象（非JSON） 默认获取为文本， isobject 决定是否对其进行 JSON.stringify
    if (isobject == true) {
        return JSON.stringify(data[objName]);
    } else {
        return data[objName].toString().replace(str_reg, '\\');
    }
}
function getJSON(jscript) {
    /* 需要传入类似于：
    [0] 第0个
    ["name"] 子属性name
    [0]["name"] 第0个的子属性 name
    */
    return eval("data" + jscript);
}
function setJSON(jscript, value) {
    /* 需要传入类似于：
    [0] 第0个
    ["name"] 子属性name
    [0]["name"] 第0个的子属性 name
    */
    eval("data" + jscript + " = " + value);
}

function setObject(objName, value) {
    var values = value;
    data[objName] = values;
}