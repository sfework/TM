
//$(document).ready(function () {
//    $('#search').click(function () {
//        var _url = url.parseParam($(this).parents('.searchbox').getdata());
//        if (_url.length > 0) {
//            window.location.href = window.location.pathname + '?' + _url;
//        }
//        else {
//            window.location.href = window.location.pathname;
//        }
//    });
//    $('#search_inter').click(function () {
//        var _url = url.parseParam($(this).parents('.searchbox').getdata());
//        if (_url.length > 0) {
//            window.location.href = window.location.pathname + '?' + _url + '&History=' + $('#History').checkbox('is checked').toString();
//        }
//        else {
//            window.location.href = window.location.pathname;
//        }
//    });
//    $('.paging .ui.dropdown').dropdown({
//        onChange: function (value, text, $choice) {
//            var URL = window.location.search;
//            URL = url.delParam(URL, 'PageSize');
//            URL += URL.indexOf('?') > -1 ? '&' : '?';
//            URL += 'PageSize=' + value;
//            window.location.href = window.location.pathname + URL;
//        }
//    });
//    //顶部菜单下拉框初始化
//    $('.Top .ui.dropdown[tabindex!="0"]').dropdown({ 'action': 'hide' });
//    control_init();
//});
//var control_init = () => {
//    //带有可清除标记的下拉框初始化
//    $('.ui.dropdown.clearable[tabindex!="0"]').dropdown({ clearable: true });
//    //其他普通下拉框初始化
//    $('.ui.dropdown[tabindex!="0"]').dropdown();

//    $('.date-ymd input').attr('readonly', true);


//    $('.date-ymd').calendar({
//        firstDayOfWeek: 1,
//        monthFirst: false,
//        type: 'date',
//        formatter: {
//            date: function (date, settings) {
//                if (!date) return '';
//                var day = date.getDate().toString();
//                day = (Array(2).join('0') + day).slice(-2);
//                var month = (date.getMonth() + 1).toString();
//                month = (Array(2).join('0') + month).slice(-2);
//                var year = date.getFullYear();
//                return year + '/' + month + '/' + day;
//            }
//        },
//        text: {
//            days: ['日', '一', '二', '三', '四', '五', '六'],
//            months: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
//            monthsShort: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
//            today: '今天',
//            now: '当前',
//            am: '上午',
//            pm: '下午'
//        }
//    });


//    //laydate.render({
//    //    elem: '.date', //指定元素
//    //    format: 'yyyy/MM/dd',
//    //    showBottom: false,
//    //    theme: '#393D49'
//    //});
//    $('.CardNo').bind("input", function () {
//        $(this).val($(this).val().replace(/[ \f\t\v]/g, '').replace(/(\d{4})(?=\d)/g, "$1  "));
//    });
//};


////异步弹窗
//class Modal {
//    constructor(url, size, id, callback) {
//        this.url = url;
//        if (!size) {
//            this.size = 'mini';
//        }
//        else {
//            this.size = size;
//        }
//        this.dom = null;
//        this.callback = callback;
//        this.id = id;
//    }
//    load() {
//        ajax.get(this.url, $.proxy(this.init, this), $.proxy(this.done, this));
//    }
//    done(r) {
//        $(this.dom).children('.header,.content,.actions').remove();
//        $(this.dom).append(r).removeClass('mini').addClass(this.size);
//        control_init();
//    }
//    init() {
//        if ($('.ui.dimmer.modals.page.transition').length === 0) {
//            $('body').append($('<div class="ui dimmer modals page transition"></div>'));
//        }
//        var html = $('<div class="ui mini modal"><i class="close icon"></i><div class="header">加载中...</div><div class="content"><div class="ui icon header" style="width:100%;margin:0;"><i class="big spinner loading icon icon"></i><div class="content" style="margin-top:25px;"><div class="sub header">请稍后，弹窗页面正在加载.</div></div></div></div><div class="actions"><div class="ui cancel black button">取消</div></div></div>');
//        if (this.id) {
//            html.attr('id', this.id);
//        }
//        var seting = {
//            allowMultiple: true, closable: false,
//            onHidden: function () {
//                $(this).remove();
//            },
//            autofocus: false
//        };
//        if (this.callback) {
//            seting.onApprove = this.callback;
//        }
//        this.dom = $(html).modal(seting).modal('show');
//    }
//}
//var modal = {
//    load: function (url, width, id, callback) { new Modal(url, width, id, callback).load(); },
//    ask: function (message, callback) {
//        if ($('.ui.dimmer.modals.page.transition').length === 0) {
//            $('body').append($('<div class="ui dimmer modals page transition"></div>'));
//        }
//        var html = $('<div class="ui mini modal"><i class="close icon"></i><div class="header"><i class="icon exclamation triangle"></i>操作确认</div><div class="content">' + message + '</div><div class="actions"><div class="ui ok green button">确定</div><div class="ui cancel black button">取消</div></div></div>');
//        var seting = {
//            allowMultiple: true, closable: false,
//            onHidden: function () {
//                $(this).remove();
//            },
//            autofocus: false
//        };
//        if (callback) {
//            seting.onApprove = callback;
//        }
//        this.dom = $(html).modal(seting).modal('show');
//    }
//};


//jQuery.fn.modal_hide = function () {
//    $(this).modal('hide');
//};

//var url = {
//    delParam: (url, ref) => {
//        if (url.indexOf(ref) === -1) {
//            return url;
//        }
//        var arr_url = url.split('?');
//        var base = arr_url[0];
//        var arr_param = arr_url[1].split('&');
//        var index = -1;
//        for (i = 0; i < arr_param.length; i++) {
//            var paired = arr_param[i].split('=');
//            if (paired[0] === ref) {

//                index = i;
//                break;
//            }
//        }
//        if (index === -1) {
//            return url;
//        } else {
//            arr_param.splice(index, 1);
//            return base + "?" + arr_param.join('&');
//        }
//    },
//    parseParam: function (params) {
//        var queryAry = [], val = undefined;
//        for (var query in params) {
//            val = params[query];
//            if (val !== '' && typeof val !== 'undefined' && val !== null) {
//                queryAry.push(query + '=' + val);
//            }
//        }
//        return queryAry.join('&');
//    },
//    load(page) {
//        var URL = window.location.search;
//        URL = this.delParam(URL, 'Page');
//        URL += URL.indexOf('?') > -1 ? '&' : '?';
//        URL += 'Page=' + page;
//        window.location.href = window.location.pathname + URL;
//    }
//};

jQuery.fn.getdata = function () {
    var objs = $(this).find('input[type="text"],input[type="file"],input[type="checkbox"],input[type="hidden"],input[type="password"],input[type="number"],textarea');
    var qid = "id", result = {};
    for (var i = 0; i < objs.length; i++) {
        var o = objs[i];
        var key = $(o).attr(qid);
        if (key !== undefined) {
            var val = null;
            if (o.nodeName === 'INPUT') {
                switch (o.type) {
                    case 'hidden':
                    case 'password':
                    case 'text':
                        val = $(o).val();
                        break;
                    case 'number':
                        val = Number($(o).val());
                        break;
                    case 'file':
                        val = o.files.length > 0 ? o.files : null;
                        break;
                    case 'checkbox':
                        val = o.checked;
                        break;
                }
            }
            if (o.nodeName === 'TEXTAREA') {
                val = $(o).val();
            }
            result[key] = val;
        }
    }
    return result;
};
$(document).ready(function () {
    $('.searchbox #search').click(function () {
        $(this).load_show();
        url.search($(this));
    });
    $('.Paging>.ui.dropdown').dropdown({
        onChange: function (value, text, e) {
            url.resize(value);
        }
    });
    init();
});
var init = function () {
    $('.projectset[tabindex!="0"]').dropdown({
        action: 'hide',
        onChange: function (value, text, $choice) {
            ajax.post('/Home/ProjectSet', { ProjectID: value }, function () {}, function (success, data) {
                if (success && data.Success) {
                    window.location.reload();
                }
            });
        }
    });
    //无操作的下拉框
    $('.ui.dropdown.noaction[tabindex!="0"]').dropdown({ 'action': 'hide' });
    //带有可清除标记的下拉框初始化
    $('.ui.dropdown.clearable[tabindex!="0"]').dropdown({ clearable: true });
    //其他普通下拉框初始化
    $('.ui.dropdown[tabindex!="0"]').dropdown();
    //带清除按钮的输入框
    $('.clearable.input > input').each((i, e) => {
        if ($(e).val().length > 0) {
            $(e).next().css('display', 'block');
        }
    });
    $('.clearable.input > input').focus(function () {
        if ($(this).val().length > 0) {
            $(this).next().css('display', 'block');
        }
    });
    $('.clearable.input > .close').click(function () {
        $(this).prev().val('');
        $(this).css('display', 'none');
    });
    $('.clearable.input > input').bind('input propertychange', function () {
        if ($(this).val().length > 0) {
            $(this).next().css('display', 'block');
        }
        else {
            $(this).next().css('display', 'none');
        }
    });
};
var ajax = new ajaxhelp();
function ajaxhelp() {
    var base = function (url, mode, data, before, done, contentType, dataType, processData) {
        var _settings = {
            //预请求的数据
            data: data,
            //指定请求连接
            url: url,
            //指定请求方式
            type: mode,
            //请求超时时间
            timeout: '30000',
            //请求内容类型
            contentType: contentType,
            //是否开启异步
            async: true,
            //是否转换为查询字符串
            processData: processData,
            //回调函数中$(this)所代表的Dom
            context: null,
            //返回的数据格式
            dataType: dataType,
            //请求前执行
            beforeSend: before,// function (xhr) { },
            //请求成功执行
            success: function (result, status, xhr) {
                //Success
                if (!result.Success) {
                    tip.error(result.ErrorMessage);
                }
                if (done !== undefined) {
                    done(true, result);
                }
            },
            //请求错误执行
            error: function (xhr, status, error) {
                tip.error(error);
                if (done !== undefined) { done(false, error); }
            },
            //请求完成执行，不管成功或失败
            complete: function (xhr, status) { }
        };
        var hasfile = false;
        for (var s in data) {
            if (data[s] && data[s].constructor.name === 'FileList') {
                hasfile = true;
                break;
            }
        }
        if (hasfile) {
            var formData = new FormData();
            for (var item in data) {
                if (data[item].constructor.name === 'FileList') {
                    var files = data[item];
                    for (var i = 0; i < files.length; i++) {
                        formData.append(item + '[]', files[i]);
                    }
                }
                else {
                    formData.append(item, data[item]);
                }
            }
            _settings.contentType = false;
            _settings.processData = false;
            _settings.traditional = true;
            _settings.data = formData;
        }
        $.ajax(_settings);
    };
    this.post = function (url, data, before, done) {
        base(url, 'POST', data, before, done, 'application/x-www-form-urlencoded', 'json', true);
    };
    this.get = function (url, before, done) {
        base(url, 'GET', null, before, done, 'text/html', 'html', true);
    };
}

jQuery.fn.load_show = function () {
    $(this).addClass('loading disabled');
};
jQuery.fn.load_hide = function () {
    $(this).removeClass('loading disabled');
};
var modal = {
    load: function (url, size, sizetp) {
        if ($('.ui.dimmer.modals.page.transition').length === 0) {
            $('body').append($('<div class="ui dimmer modals page transition"></div>'));
        }
        var html = $('<div class="ui mini modal"><i class="close icon"></i><div class="header">加载中...</div><div class="content"><div class="ui icon header" style="width:100%;margin:0;"><i class="big spinner loading icon icon"></i><div class="content" style="margin-top:25px;"><div class="sub header">请稍后，弹窗页面正在加载.</div></div></div></div><div class="actions"><div class="ui cancel black button">取消</div></div></div>');
        ajax.get(url, function () {
            var seting = {
                allowMultiple: true, closable: false,
                onHidden: function () {
                    $(this).remove();
                },
                autofocus: false
            };
            $(html).modal(seting).modal('show');
        }, function (success, data) {
            if (success) {
                try {
                    data = JSON.parse(data);
                    $(html).modal('hide');
                    tip.error(data.ErrorMessage);
                }
                catch{
                    if (sizetp) {
                        html.html(data).removeClass('mini').addClass(sizetp);
                    }
                    if (size) {
                        html.html(data).removeClass('mini').css('width', size + 'px');
                    }
                    init();
                }
            } else {
                tip.error('弹窗内容加载失败，请检查异常原因！');
            }
        });
    },
    ask: function (message, callback) {
        if ($('.ui.dimmer.modals.page.transition').length === 0) {
            $('body').append($('<div class="ui dimmer modals page transition"></div>'));
        }
        var html = $('<div class="ui inverted mini modal"><i class="close icon"></i><div class="header"><i class="icon exclamation triangle"></i>操作确认</div><div class="content">' + message + '</div><div class="actions"><div class="ui ok red button">确定</div><div class="ui cancel black button">取消</div></div></div>');
        var seting = {
            allowMultiple: true, closable: false, inverted: true,
            onHidden: function () {
                $(this).remove();
            },
            autofocus: false
        };
        if (callback) {
            seting.onApprove = callback;
        }
        this.dom = $(html).modal(seting).modal('show');
    }
};
var tip = {
    error: (message) => {
        $('body').toast({
            class: 'error',
            position: 'bottom center',
            displayTime: 3000,
            message: message
        });
    },
    success: (message) => {
        $('body').toast({
            class: 'success',
            position: 'bottom center',
            displayTime: 2000,
            message: message
        });
    }
};
function clone(Obj) {
    var buf;
    if (Obj instanceof Array) {
        buf = [];
        var i = Obj.length;
        while (i--) {
            buf[i] = clone(Obj[i]);
        }
        return buf;
    }
    else if (Obj instanceof Object) {
        buf = {};
        for (var k in Obj) {
            buf[k] = clone(Obj[k]);
        }
        return buf;
    } else {
        return Obj;
    }
}
var url = {
    parseParam: function (params) {
        var queryAry = [], val = undefined;
        for (var query in params) {
            val = params[query];
            if (val !== '' && typeof val !== 'undefined' && val !== null) {
                queryAry.push(query + '=' + val);
            }
        }
        return queryAry.join('&');
    },
    replaceParamVal: function (oUrl, paramName, replaceWith) {
        if (oUrl.length < 1) {
            oUrl = "?" + paramName + '=' + replaceWith;
            return oUrl;
        }
        var reg = new RegExp("(^|&)" + paramName + "=([^&]*)(&|$)", "i");
        var r = oUrl.match(reg);
        if (r === null) {
            oUrl = oUrl + "&" + paramName + '=' + replaceWith;
            return oUrl;
        }
        else {
            var re = eval('/(' + paramName + '=)([^&]*)/gi');
            var nUrl = oUrl.replace(re, paramName + '=' + replaceWith);
            if (nUrl.substr(0, 1) !== '?') {
                nUrl = '?' + nUrl;
            }
            return nUrl;
        }
    },
    search: function (el) {
        var data = $(el).parents('.searchbox').getdata();
        var tag = $(el).parents('.searchbox').data('tag');
        var pagesize = $('.Paging[data-tag="' + tag + '"]').data('pagesize');
        data['Page'] = 1;
        data['PageSize'] = pagesize;
        var _url = "?" + url.parseParam(data);
        window.location.href = window.location.pathname + _url;
    },
    load: function (p) {
        if (event) {
            var target = event.currentTarget;
            $(target).load_show();
        }
        var oUrl = window.location.search.substr(1);
        oUrl = url.replaceParamVal(oUrl, 'Page', p);
        window.location.href = window.location.pathname + oUrl;
    },
    resize: function (s) {
        var oUrl = window.location.search.substr(1);
        console.log(oUrl);
        oUrl = url.replaceParamVal(oUrl, 'Page', '1');
        console.log(oUrl);
        oUrl = url.replaceParamVal(oUrl, 'PageSize', s);
        console.log(oUrl);
        window.location.href = window.location.pathname + oUrl;
    }
};
function copyToClipboard(s) {
    if (window.clipboardData) {
        window.clipboardData.setData('text', s);
    } else {
        (function (s) {
            document.oncopy = function (e) {
                e.clipboardData.setData('text', s);
                e.preventDefault();
                document.oncopy = null;
            };
        })(s);
        document.execCommand('Copy');
    }
}