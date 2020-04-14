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
    $('.paging>.ui.dropdown').dropdown({
        onChange: function (value, text, e) {
            url.resize(value);
        }
    });
    init();
});
var init = function () {
    //项目设置下拉框
    $('.projectset[tabindex!="0"]').dropdown({
        action: 'hide',
        onChange: function (value, text, $choice) {
            ajax.post('/Home/ProjectSet', { ProjectID: value }, function () { }, function (success, data) {
                if (success && data.Success) {
                    window.location.href = window.location.pathname;
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
    //左栏菜单头像部分的隐藏操作
    $('.menubox>.blurring>.info').dimmer({ on: 'hover' });
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
        var html = $('<div class="ui mini modal"><i class="close icon"></i><div class="header"><i class="icon exclamation triangle"></i>操作确认</div><div class="content">' + message + '</div><div class="actions"><div class="ui right labeled icon button negative positive">确定<i class="checkmark icon"></i></div><div class="ui button green cancel">取消</div></div></div>');
        var seting = {
            allowMultiple: true, closable: false,
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
    replaceParamVal: function (paramName, replaceWith) {
        var ourl = window.location.search;
        if (ourl) {
            ourl = ourl.replace('?', '');
        }
        else {
            ourl = paramName + '=' + replaceWith;
            return "?" + ourl;
        }
        var reg = new RegExp("(^|&)" + paramName + "=([^&]*)(&|$)", "i");
        var r = ourl.match(reg);
        if (r === null) {
            ourl = ourl + "&" + paramName + '=' + replaceWith;
            return '?' + ourl;
        }
        else {
            var re = eval('/(' + paramName + '=)([^&]*)/gi');
            var nUrl = ourl.replace(re, paramName + '=' + replaceWith);
            return '?' + nUrl;
        }
    },
    search: function (el) {
        var data = $(el).parents('.searchbox').getdata();
        data = url.parseParam(data);
        data = data.replace('#', '%23');
        var _url = "";
        if (data) {
            _url = "?" + data;
        }
        window.location.href = window.location.pathname + _url;
    },
    load: function (p) {
        if (event) {
            var target = event.currentTarget;
            $(target).load_show();
        }
        oUrl = url.replaceParamVal('Page', p);
        window.location.href = window.location.pathname + oUrl;
    },
    resize: function (s) {
        oUrl = url.replaceParamVal('Page', '1');
        oUrl = url.replaceParamVal('PageSize', s);
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