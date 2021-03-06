(function (window) {

    /*************************************************
        Simple Dropdown jQuery plugin (handmade)
    **************************************************/
    (function ($) {

        // helper function for strings
        // add '...' after len chars
        String.prototype.strmin = function (len) {
            if (this && (this.length - len) > 3)
                return this.substr(0, len) + '...';
            return this.toString();
        }

        // fix for IE8
        // no method "trim" for string objects
        if (typeof String.prototype.trim !== 'function') {
            String.prototype.trim = function () {
                return this.replace(/^\s+|\s+$/g, '');
            }
        }

        var $simpleDropdown = null;

        var htmlDropdown = '\
            <div class="simple-dropdown-current"></div>\
            <div class="simple-dropdown-list-ww">\
                <div class="simple-dropdown-list-w">\
                    <div class="simple-dropdown-input-ww empty">\
                        <div class="simple-dropdown-input-w">\
                            <div class="simple-dropdown-input">\
                                <input type="text" class="simple-filter" placeholder="Filter list..." />\
                            </div>\
                            <div class="simple-dropdown-right">\
                                <i class="icon-remove"></i>\
                            </div>\
                        </div>\
                    </div>\
                    <div class="simple-dropdown-list-border">\
                        <div class="simple-dropdown-list"></div>\
                    </div>\
                </div>\
            </div>';

        var CreateListItem = function (data, clickCallback) {
            var $this = this;
            var $elem = $('<div class="simple-dropdown-item" data-item-id="' + data.id + '">' + 
                '<span></span><b class="caret"></b>' +
                '</div>');

            $elem.find('span')
                .text(data.name.strmin(25))
                .tooltip({
                    title: data.name
                });

            $elem.click(clickCallback);

            return $elem;
        }

        /*___________________________
            FILL LIST AJAX CALLBACKS
        _____________________________*/
        var onFillSuccess = function ($this, options, items) {
            $.each(items, function (i, v) {
                $this.addListItemElement(v);
            });
        }

        var onFillError = function ($this, options) {
        }
        /*___________________________
            END.
        _____________________________*/

        var fillList = function ($this, options) {

            $.ajax(options.url, {
                type: 'GET',
                datatype: 'json',
                success: function (data) {
                    onFillSuccess($this, options, data)
                },
                error: function () {
                    onFillError($this, options);
                }
            });
        };

        $.fn.simpleDropdown = function (options) {
            var $this = this;

            /* private property */
            var items = [];

            /* for exterior use (closure context) */
            $simpleDropdown = $this;

            $this.addClass('simple-dropdown');
            $this.append(htmlDropdown);

            var $hiddenInput = $('<input type="hidden" name="' + options.inputName + '" value="' + options.selectedID + '" />');

            $this.append($hiddenInput);

            var currentName = 'Loading...';
            if (options.selectedID == -1) {
                currentName = options.defaultText;
            }

            var $current = $this.find('.simple-dropdown-current').append(CreateListItem({
                    type: 'Pie Chart',
                    name: currentName,
                    scheduled: true
                }))

            /* open-close the dropdown */
            .click(function (event) {
                event.stopPropagation();

                $this.toggleClass('open');
            });

            /* click outside closes the dropdown */
            $('html').click(function () {
                $this.removeClass('open');
            });

            /* click on filter input does nothing */
            var $inputWrapper = $this.find('.simple-dropdown-input-ww').click(function (e) {
                e.stopPropagation();
            });

            var $input = $inputWrapper.find('input');

            var dropdownList = $this.find('.simple-dropdown-list');

            var filterTimer = null;

            /* filter the dropdown by some string */
            var filterList = function () {
                
                filterTimer && clearTimeout(filterTimer);
                filterTimer = setTimeout(filterTimerCbk, 500);
            }
            /* used inside filterList as timer callback */
            var filterTimerCbk = function () {
                var regExPattern = null,
                    emptyList = true,
                    str = $input.val();

                console.log('Filtering by \'' + str + '\' ....');


                if (!str || str.trim() == '') {
                    str = '.*';
                }

                regExPattern = new RegExp(str, 'i');

                $.each(items, function (index, listItem) {
                    if (regExPattern.test(listItem.item.name)) {
                        listItem.$item.removeClass('hidden');
                        emptyList = false;
                    } else {
                        listItem.$item.addClass('hidden');
                    }
                });

                if (!emptyList) {
                    dropdownList.removeClass('hidden');
                } else {
                    dropdownList.addClass('hidden');
                }
            }

            $input.keypress(function (e) {
                var inputVal = $input.val();

                if (inputVal == '') {
                    $inputWrapper.addClass('empty');
                } else {
                    $inputWrapper.removeClass('empty');
                }

                filterList(inputVal);
            });

            /* click on X clears the filter text input */
            var clearFilter = function () {
                $input.val('');
                $inputWrapper.addClass('empty');
                filterTimerCbk(); // empty filter => all pass
            };

            $this.find('.simple-dropdown-right').click(clearFilter);


            /* Esc clears the filter text input */
            $input.keyup(function (e) {
                switch (e.keyCode)
                {
                    case 8:
                        console.log('backspace hit');
                        filterList($input.val());
                        break;
                    case 27:
                        clearFilter();
                        break;
                }
            });

            var selectItem = function (listItem, item) {
                var newItem = null;
                    
                $hiddenInput.val(item.id).trigger('change');

                listItem.tooltip('hide');
                newItem = listItem.clone();
                newItem.tooltip({ title: item.name });
                $current.children().remove();
                $current
                    .append(newItem);
                console.log('Selected item: ' + item.name);
            }

            /* public methods */
            $this.addListItemElement = function (item) {
                var listItem = CreateListItem({
                    id: item.id,
                    name: item.name,
                }, function () { // item click callback
                    selectItem($(this), item);
                });

                if (item.id == options.selectedID) {
                    selectItem(listItem, item);
                }

                /* remember this details for 
                later filtering */
                items.push({
                    item: item,
                    $item: listItem
                });

                dropdownList.append(listItem);
            }

            fillList($this, options);

            return $this;
        }
    })(jQuery);
    /**********************************************************
        End: Simple Dropdown jQuery plugin
    ***********************************************************/
})(window);
