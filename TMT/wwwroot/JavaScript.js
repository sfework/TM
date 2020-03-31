
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

////网络请求及处理
//var ajax = {
//    base: function (url, mode, data, before, done, contentType, dataType, processData) {
//        var _settings = {
//            //预请求的数据
//            data: data,
//            //指定请求连接
//            url: url,
//            //指定请求方式
//            type: mode,
//            //请求超时时间
//            timeout: '30000',
//            //请求内容类型
//            contentType: contentType,
//            //是否开启异步
//            async: true,
//            //是否转换为查询字符串
//            processData: processData,
//            //回调函数中$(this)所代表的Dom
//            context: null,
//            //返回的数据格式
//            dataType: dataType,
//            //请求前执行
//            beforeSend: before,// function (xhr) { },
//            //请求成功执行
//            success: function (result, status, xhr) { if (done !== undefined) { done(result); } },
//            //请求错误执行
//            error: function (xhr, status, error) {
//                tip.error(error);
//                //if (done !== undefined) { done(false, error); }
//            },
//            //请求完成执行，不管成功或失败
//            complete: function (xhr, status) { }
//        };
//        $.ajax(_settings);
//    },
//    post: function (url, data, before, done) {
//        this.base(url, 'POST', data, before, done, 'application/x-www-form-urlencoded', 'json', true);
//    },
//    get: function (url, before, done) {
//        this.base(url, 'GET', null, before, done, 'text/html', 'html', true);
//    },
//    upfile: function () {
//        //var data = formData;
//        this.base(url, null, before, done, false, 'json', false);
//    }
//    //ParseParam: function (param, key) {
//    //    var paramStr = "";
//    //    if (encodeURIComponent(param) && (param instanceof String || param instanceof Number || param instanceof Boolean)) {
//    //        paramStr += "&" + key + "=" + encodeURIComponent(param);
//    //    } else {
//    //        $.each(param, function (i) {
//    //            var k = key === null ? i : key + (param instanceof Array ? "[" + i + "]" : "." + i);
//    //            if (ajax.ParseParam(this, k)) {
//    //                paramStr += '&' + ajax.ParseParam(this, k);
//    //            }
//    //        });
//    //    }
//    //    return paramStr.substr(1);
//    //}
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

//jQuery.fn.getdata = function () {
//    var objs = $(this).find('input[type="text"],input[type="file"],input[type="checkbox"],input[type="hidden"],input[type="password"],input[type="number"],textarea');
//    var qid = "name", result = {};
//    for (var i = 0; i < objs.length; i++) {
//        var o = objs[i];
//        var key = $(o).attr(qid);
//        if (key !== undefined) {
//            var val = null;
//            if (o.nodeName === 'INPUT') {
//                switch (o.type) {
//                    case 'hidden':
//                    case 'password':
//                    case 'text':
//                        val = $(o).val();
//                        break;
//                    case 'number':
//                        val = Number($(o).val());
//                        break;
//                    case 'file':
//                        val = o.files.length > 0 ? o.files : null;
//                        break;
//                    case 'checkbox':
//                        val = o.checked;
//                        break;
//                }
//            }
//            if (o.nodeName === 'TEXTAREA') {
//                val = $(o).val();
//            }
//            result[key] = val;
//        }
//    }
//    return result;
//};

//jQuery.fn.show_load = function () {
//    $(this).addClass('loading disabled');
//};
//jQuery.fn.hide_load = function () {
//    $(this).removeClass('loading disabled');
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


//var tip = {
//    error: (message) => {
//        $('body').toast({
//            class: 'error',
//            position: 'bottom center',
//            displayTime: 3000,
//            message: message
//        });
//    },
//    success: (message) => {
//        $('body').toast({
//            class: 'success',
//            position: 'bottom center',
//            displayTime: 2000,
//            message: message
//        });
//    }
//};