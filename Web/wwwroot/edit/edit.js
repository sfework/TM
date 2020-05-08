class editor {
    constructor(element, option) {
        if ($(element).hasClass('editor-container') && $(element).data('context')) {
            var context = $(element).data('context');
            return context;
        }
        this.element = $(element).addClass('editor-container').data('context', this);
        this.option = option;
        if (this.element.html().trim()) {
            this.element.html(this.element.html());
            this.element.children('.item_content').each((i, e) => {
                this.components.bind($(e), this.option.edit);
            });
        } else {
            this.components.new.text(null, true);
        }
        this.ready();
    }
    components = {
        new: {
            text: (target, focus) => {
                var el = $('<div class="item_content" type="text"><div class="content"></div></div>');
                return this.components.insert(target, el, focus);
            },
            header: (target, focus) => {
                var el = $('<div class="item_content" type="header"><div class="content h4"></div></div>');
                return this.components.insert(target, el, focus);
            },
            list: (target, focus) => {
                var el = $('<div class="item_content" type="list"><ul><li></li></ul></div>');
                return this.components.insert(target, el, focus);
            },
            code: (target, focus) => {
                var el = $('<div class="item_content" type="code"><div class="content"></div></div>');
                return this.components.insert(target, el, focus);
            },
            quote: (target, focus) => {
                var el = $('<div class="item_content" type="quote"><div class="content"></div></div>');
                return this.components.insert(target, el, focus);
            },
            warning: (target, focus) => {
                var el = $('<div class="item_content" type="warning"><div class="content"></div></div>');
                return this.components.insert(target, el, focus);
            },
            divider: (target, focus) => {
                var el = $('<div class="item_content" type="divider"><div class="content"></div></div>');
                return this.components.insert(target, el, focus);
            },
            table: (target, focus) => {
                var el = $('<div class="item_content" type="table"><table><tr><td></td><td></td></tr><tr><td></td><td></td></tr></table></div>');
                return this.components.insert(target, el, focus);
            },
            image: (target, img, title, focus) => {
                var el = $('<div class="item_content" type="image"><div class="imagebox"><img src="' + img + '" /><input type="text" value="' + title + '" /><span></span></div></div>');
                return this.components.insert(target, el, focus);
            },
            upload: (target, url, title, focus) => {
                var el = $('<div class="item_content" type="upload"><a class="download" href="' + url + '" download="' + title + '">' + title + '</a></div>');
                return this.components.insert(target, el, focus);
            },
        },
        insert: (target, el, focus) => {
            var tp = el.attr('type');
            el = this.components.bind(el, this.option.edit);
            if (target) {
                target.after(el);
            } else {
                this.element.append(el);
            }
            if (tp !== 'text' && el.next().length < 1) {
                this.components.new.text(el, false);
            }
            if (this.option.edit && focus) {
                this.components.active(el);
            }
            return el;
        },
        upload: (file, successcallback, errorcallback) => {
            console.log(file);
            var data = new FormData();
            data.append(file.name, file);
            $.ajax({
                data: data,
                url: this.option.upload.url,
                type: 'POST',
                timeout: '30000',
                contentType: false,
                processData: false,
                success: (result, status, xhr) => {
                    successcallback(result);
                },
                error: (xhr, status, error) => { errorcallback(error); }
            });
        },
        bind: (el, edit) => {
            var tp = el.attr('type');
            el.removeClass("view");
            if (edit) {
                el = el.prepend(this.components.createtool(el));
                el.click((e) => {
                    var context = $(e.target).get_edit_context();
                    context.components.active(el);
                });
                switch (tp) {
                    case 'text':
                        el.children('.content').bind('input', function(e) {
                            if (el.next().length < 1) {
                                var context = el.get_edit_context();
                                context.components.new.text(el, false);
                            }
                        });
                        el.children('.content').attr('contenteditable', 'plaintext-only');
                        this.components.inlinecontrol(el);
                        break;
                    case 'header':
                    case 'code':
                    case 'quote':
                    case 'warning':
                        el.children('.content').attr('contenteditable', 'plaintext-only');
                        this.components.inlinecontrol(el);
                        break;
                    case 'table':
                        el.find('table>tbody>tr>td').unbind('click').bind('click', (e) => {
                            $(e.target).parents('table').data('el', $(e.target));
                        });
                        el.children('table').attr('contenteditable', 'plaintext-only');
                        break;
                    case 'list':
                        el.children('ul').attr('contenteditable', 'true');
                        break;
                    case 'image':
                        el.find('.imagebox>span').mousedown((e) => {
                            var initX = e.pageX;
                            var img = $(e.target).siblings('img');
                            var imgw = img.width();
                            $(document).bind('mousemove', (e) => {
                                var nw = imgw - (initX - e.pageX);
                                if (nw < 200) {
                                    nw = 200;
                                }
                                img.width(nw);
                            }).mouseup((e) => {
                                $(document).unbind('mousemove').unbind('mouseup');
                            });
                        });
                        el.find('.imagebox>input').removeAttr('readonly');
                        break;
                }
            } else {
                el.addClass("view").removeClass('active').unbind('click').children('.tools').remove();
                switch (tp) {
                    case 'text':
                    case 'header':
                    case 'code':
                    case 'quote':
                    case 'warning':
                        el.children('.content').unbind('mouseup').removeAttr('contenteditable');
                        break;
                    case 'list':
                        el.children('ul').removeAttr('contenteditable');
                        break;
                    case 'table':
                        el.children('table').removeAttr('contenteditable');
                        break;
                    case 'image':
                        el.find('.imagebox>input').attr('readonly', 'readonly');
                        break;
                }
            }
            return el;
        },
        inlinecontrol: (el) => {
            el.children('.content').unbind('mouseup').bind('mouseup', (e) => {
                var selection = window.getSelection();
                var range = selection.getRangeAt(0);
                if (!range.collapsed) {
                    $(e.target).parents('.item_content').children('.tools').children('.text.extend').data('curr_range', range);
                }
                if (['blod', 'careful', 'inlinecode', 'marker'].includes(e.target.className)) {
                    $(e.target).parents('.item_content').find('.tools>.text.extend>ul>li.' + e.target.className).addClass('active').siblings().removeClass('active');
                } else {
                    $(e.target).parents('.item_content').find('.tools>.text.extend>ul>li').removeClass('active');
                }
            });
        },
        defocus: () => {
            this.element.children('.item_content').each((i, e) => {
                if ($(e).hasClass('active')) {
                    $(e).removeClass('active');
                    $(e).children('.tools').removeClass('active');
                    $(e).find('.tools>.control>ul>li').removeClass('confirm');
                }
            });
        },
        active: (el) => {
            this.components.defocus();
            el = el.addClass('active');
            var tools = el.children('.tools').addClass('active');
            var tp = el.attr('type');
            var basictool = tools.find('.basic>ul>li.' + tp);
            basictool.addClass('active').siblings().removeClass('active');
            switch (tp) {
                case 'text':
                case 'code':
                case 'quote':
                case 'warning':
                    el.children('.content').focus();
                    break;
                case 'header':
                    el.children('.content').focus();
                    var hx = el.children('.content')[0].className.replace('content', '').replace('active', '').trim();
                    var extendtool = tools.find('.extend>ul>li.' + hx);
                    extendtool.addClass('active').siblings().removeClass('active');
                    break;
                case 'list':
                    el.children('ul').focus();
                    break;
            }
            return el;
        },
        createtool: (el) => {
            var tp = el.attr('type');
            var toolbox = $('<div class="tools"></div>');
            var basic = $('<div class="basic">\
                            <ul>\
                                <li class="text">&#xe101;</li>\
                                <li class="header">&#xe102;</li>\
                                <li class="list">&#xe103;</li>\
                                <li class="code">&#xe104;</li>\
                                <li class="quote">&#xe105;</li>\
                                <li class="warning">&#xe106;</li>\
                                <li class="image">&#xe107;</li>\
                                <li class="upload">&#xe108;</li>\
                                <li class="table">&#xe109;</li>\
                                <li class="divider">&#xe110;</li>\
                            </ul>\
                        </div>');
            var control = $('<div class="control">\
                            <ul>\
                                <li class="add">&#xe201;</li>\
                                <li class="up">&#xe202;</li>\
                                <li class="down">&#xe203;</li>\
                                <li class="del">&#xe204;</li>\
                            </ul>\
                        </div>');
            var text_extend = $('<div class="text extend">\
                                    <ul>\
                                        <li class="blod">&#xe301;</li>\
                                        <li class="careful">&#xe302;</li>\
                                        <li class="inlinecode">&#xe303;</li>\
                                        <li class="marker">&#xe304;</li>\
                                    </ul>\
                                </div>');
            var header_extend = $('<div class="header extend">\
                                    <ul>\
                                    <li class="h1">&#xe401;</li>\
                                    <li class="h2">&#xe402;</li>\
                                    <li class="h3">&#xe403;</li>\
                                    <li class="h4">&#xe404;</li>\
                                    </ul>\
                                    </div>');

            var table_extend = $('<div class="table extend">\
                                    <ul>\
                                        <li class="top_add_row">&#xe901;</li>\
                                        <li class="bottom_add_row">&#xe902;</li>\
                                        <li class="left_add_col">&#xe903;</li>\
                                        <li class="right_add_col">&#xe904;</li>\
                                        <li class="del_row">&#xe905;</li>\
                                        <li class="del_col">&#xe906;</li>\
                                    </ul>\
                                </div>');
            text_extend.find('ul>li').click((e) => {
                e.stopPropagation();
                var action = e.target.className;
                var range = $(e.target).parents('.text.extend').data('curr_range');
                if (action.indexOf('active') > -1) {
                    console.log(range.extractContents());
                    range.insertNode(range.extractContents());
                    return;
                }
                var txt = range.extractContents().textContent
                range.deleteContents();
                var bold = document.createElement("span");
                switch (action) {
                    case 'blod':
                        document.execCommand('ForeColor', false, 'red');
                        bold.className = 'blod';
                        break;
                    case 'careful':
                        bold.className = 'careful';
                        break;
                    case 'inlinecode':
                        bold.className = 'inlinecode';
                        break;
                    case 'marker':
                        bold.className = 'marker';
                        break;
                }
                bold.innerHTML = txt;
                range.insertNode(bold);
                var selection = window.getSelection();
                selection.removeAllRanges();
                selection.addRange(range);
            });
            header_extend.find('ul>li').click((e) => {
                e.stopPropagation();
                var item = $(e.target).get_item_content();
                $(e.target).addClass('active').siblings().removeClass('active');
                var target = item.children('.content');
                target.removeClass('h1 h2 h3 h4').addClass(e.target.className);
            });
            table_extend.find('ul>li').click((e) => {
                e.stopPropagation();
                var action = e.target.className;
                var item = $(e.target).get_item_content();
                var td = item.children('table').data('el');
                if (!td) { return; }
                var rowcount = td.parent().parent().children().length,
                    colcount = td.parent().children().length,
                    row_html = '<tr>';
                for (var i = 0; i < colcount; i++) {
                    row_html += "<td></td>"
                }
                row_html += "</tr>";
                var index = td.index();
                switch (action) {
                    case 'top_add_row':
                        td.parent().before(row_html);

                        break;
                    case 'bottom_add_row':
                        td.parent().after(row_html);
                        break;
                    case 'left_add_col':
                        td.parent().parent().children().each((i, o) => {
                            if (index === 0) {
                                $(o).prepend('<td></td>');
                            } else if (index > 0) {
                                $(o).children().eq(index - 1).after('<td></td>')
                            }
                        });
                        break;
                    case 'right_add_col':
                        td.parent().parent().children().each((i, o) => {
                            if (index + 1 >= colcount) {
                                $(o).append('<td></td>');
                            } else {
                                $(o).children().eq(index).after('<td></td>')
                            }
                        });
                        break;
                    case 'del_row':
                        if (rowcount > 1) {
                            td.parent().remove();
                        }
                        break;
                    case 'del_col':
                        if (colcount > 1) {
                            td.parent().parent().children().each((i, o) => {
                                $(o).children().eq(index).remove();
                            });
                        }
                        break;
                }
                item.find('table>tbody>tr>td').unbind('click').bind('click', (e) => {
                    $(e.target).parents('table').data('el', $(e.target));
                });
            });
            control.find('ul>li').click((e) => {
                e.stopPropagation();
                var action = e.target.className;
                var item = $(e.target).get_item_content();
                switch (action) {
                    case 'add':
                        this.components.new.text(item, true);
                        break;
                    case 'up':
                        item.prev().before(item);
                        break;
                    case 'down':
                        item.next().after(item);
                        break;
                    case 'del':
                        if (item.siblings().length > 0) {
                            $(e.target).addClass('confirm');
                        }
                        break;
                    case 'del confirm':
                        item.remove();
                        break;
                }
            });
            basic.find('ul>li').click((e) => {
                e.stopPropagation();
                var action = e.target.className;
                var item = $(e.target).get_item_content();
                var context = item.get_edit_context();
                switch (action) {
                    case 'text':
                        context.components.new.text(item, true);
                        break;
                    case 'header':
                        context.components.new.header(item, true);
                        break;
                    case 'list':
                        context.components.new.list(item, true);
                        break;
                    case 'code':
                        context.components.new.code(item, true);
                        break;
                    case 'quote':
                        context.components.new.quote(item, true);
                        break;
                    case 'warning':
                        context.components.new.warning(item, true);
                        break;
                    case 'table':
                        context.components.new.table(item, true);
                        break;
                    case 'divider':
                        context.components.new.divider(item, true);
                        break;
                    case 'image':
                    case 'image active':
                        var image_input = $('<input type="file" accept="image/*">');
                        image_input.data('context', context).data('item', item);
                        image_input.change((e) => {
                            context.components.upload(e.target.files[0], (result) => {
                                var context = $(e.target).data('context');
                                var item = $(e.target).data('item');
                                context.components.new.image(item, result.Result.path, result.Result.name, true);
                                item.remove();
                                if (context.option.upload.success) {
                                    context.option.upload.success(result);
                                }
                            }, (error) => {
                                if (context.option.upload.error) {
                                    context.option.upload.error(error);
                                }
                            })
                        });
                        image_input.click();
                        return;
                    case 'upload':
                    case 'upload active':
                        var upload_input = $('<input type="file" accept="*/*">');
                        upload_input.data('context', context).data('item', item);
                        upload_input.change((e) => {
                            context.components.upload(e.target.files[0], (result) => {
                                var context = $(e.target).data('context');
                                var item = $(e.target).data('item');
                                context.components.new.upload(item, result.Result.path, result.Result.name, true);
                                item.remove();
                                if (context.option.upload.success) {
                                    context.option.upload.success(result);
                                }
                            }, (error) => {
                                if (context.option.upload.error) {
                                    context.option.upload.error(error);
                                }
                            })
                        });
                        upload_input.click();
                        return;
                    default:
                        return;
                }
                console.log(action);
                item.remove();
            });
            toolbox.prepend(basic)
            if (['text', 'header', 'code', 'quote', 'warning'].includes(tp)) {
                //toolbox.prepend(text_extend);
                if (tp === 'header') {
                    toolbox.prepend(header_extend);
                }
            } else if (tp === 'table') {
                toolbox.prepend(table_extend);
            }
            toolbox.prepend(control);
            return toolbox;
        }
    }
    ready() {
        $(document).click(function(e) {
            if (!$(e.target).hasClass('item_content') && $(e.target).parents('.item_content').length < 1) {
                $('.editor-container').each((i, e) => {
                    var context = $(e).get_edit_context();
                    context.components.defocus();
                });
            }
        });
    }
    toggleState() {
        if (this.option.edit) {
            this.option.edit = false;
        } else {
            this.option.edit = true;
        }
        this.element.children('.item_content').each((i, e) => {
            this.components.bind($(e), this.option.edit);
        });
        if (this.option.edit && this.element.children('.item_content').length === 1) {
            this.components.active(this.element.children('.item_content:first-child'));
        }
    }
    get() {
        var html = '';
        this.element.children('.item_content').each((i, e) => {
            var el = $(e).clone();
            var context = $(e).get_edit_context();
            el = context.components.bind(el, false);
            //el.removeClass('active').children('.tools').remove()
            html += el.prop('outerHTML');
        });
        return html;
    }
}

$.fn.extend({
    get_edit_context: function() {
        var context = this.parents('.editor-container').data('context');
        if (!context) {
            context = this.data('context');
        }
        return context;
    },
    get_item_content: function() {
        var context = this.parents('.item_content');
        if (context.length < 1) {
            context = this;
        }
        return context;
    }
});