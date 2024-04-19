/**
 * Author: Andrew Terpolovskyi
 * Date: 27.06.13
 * Time: 09:00 UTC +0
 * Copyright 2013 by Andrew Terpolovskyi
 */

jQuery.fn.extend({
    live: function (event, callback) {
        if (this.selector) {
            jQuery(document).on(event, this.selector, callback);
        }
    }
});

(function ($) {
    $.fn.JLogicWizard = function (settings) {
        function JLogicWizard(element, s) {
            var wizard = this, settings, prev, next, welcome, done, anchor, current, steps, links, states, range,
                parentClass, classSteps, navHideOnSteps, titleText, titleBlock;

            Array.prototype.indexOf = function (obj, start) {
                for (var i = (start || 0), j = this.length; i < j; i++) {
                    if (this[i] === obj) { return i; }
                }
                return -1;
            }

            function initialise(s) {
                settings = s;
                prev = settings.prevButton;
                next = settings.nextButton;
                anchor = $(settings.navBlock);
                current = settings.selected;
                parentClass = settings.content.parentClass;
                classSteps = settings.content.steps;
                navHideOnSteps = settings.navHideOnSteps;
                titleText = settings.stepTitle.text;
                titleBlock = settings.stepTitle.output;
                getSteps();
                range = new Array(0, steps.length);
                element.addClass("JLWizard");
                setStep(current);

                if (prev) {
                    $(prev).live('click', function () {
                        if (ValidateStep(current - 1)) {
                            onLeaveStep();
                            if (current > range[0]) {
                                $(prev).removeClass("disabled");
                                current -= 1;
                                setStep(current);
                            }
                            else {
                                $(prev).addClass("disabled");
                            }
                        }
                        return false;
                    });
                }
                if (next) {
                    $(next).live('click', function () {
                        if (ValidateStep(current + 1)) {
                            onLeaveStep();
                            if (current + 1 < range[1]) {
                                $(next).removeClass("disabled");
                                current += 1;
                                setStep(current);
                            }
                            else {
                                // $(next).addClass("disabled");
                            }

                        }
                        return false;
                    });
                }
                anchor.children("a").click(function () {
                    var index = $(this).index();
                    index = states.indexOf(index);
                    if (index != -1) {
                        var temp_index = get_next(index);
                        if (temp_index <= range[1])
                            index = temp_index;
                        setStep(index);
                    }
                    return false;
                })
            }

            function get_next(i) {
                if ($.inArray(i, navHideOnSteps) != -1) {
                    return get_next(i + 1);
                }
                else {
                    return i;
                }
            }

            function getCompletedOn(correction) {
                var c = correction || 0;
                var number = states[current];
                var index = states.indexOf(number);
                var count = 0;
                for (var i in states) {
                    if (states[i] == number) { count += 1 }
                }
                var already = current + c - index;
                var perc = already * 100 / count;
                return Math.round(perc * 10) / 10;
            }

            function getSteps() {
                steps = new Array();
                links = new Array();
                states = new Array();
                var object = '', counter = 0;
                anchor.children("a").each(function (i, obj) {
                    object = $(obj).attr("href")
                    if (object.charAt(0) == '#') {
                        object = object.replace(/^#/, '');
                        object = "[id^=" + object + "]";
                    }
                    $(object).each(function (j, inherit_obj) {
                        steps.push($(inherit_obj));
                        states.push(counter);
                    });
                    counter += 1;
                    links.push($(obj));
                });
            }

            function defaultState() {
                anchor.children("a").removeClass("selected").show();
                if (titleText) {
                    $(titleText).hide();
                    $(titleBlock).empty();
                }
                $.each(steps, function (index, obj) {
                    $(obj).hide().addClass("content");
                });
            }

            function loadStep() {
                if (settings.onLoadStepHandler == false) {
                    return true;
                }
                else if (typeof settings.onLoadStepHandler === "function") {
                    var completedOn = getCompletedOn();
                    return settings.onLoadStepHandler.call(this, element, links[states[current]], current, completedOn);
                }
                else {
                    throw "load handler must be a function";
                }
            }

            function onLeaveStep() {
                if (settings.onLeaveHandler == false) {
                    return true;
                }
                else if (typeof settings.onLeaveHandler === "function") {
                    var completedOn = getCompletedOn(1);
                    return settings.onLeaveHandler.call(this, element, links[states[current]], current, completedOn);
                }
                else {
                    throw "load handler must be a function";
                }
            }

            function ValidateStep(next) {
                if (settings.validateHandler == false) {
                    return true;
                }
                else if (typeof settings.validateHandler === "function") {
                
                    return settings.validateHandler.call(this, element, current, next);
                }
                else {
                    throw "validation handler must be a function";
                }
            }

            function Show() {
                if (titleText) {
                    $(titleBlock).html(steps[current].find(titleText).html());
                }
                if (parentClass) {
                    element.removeClass(parentClass);
                    if ($.inArray(current, classSteps) != -1) {
                        element.addClass(parentClass);
                    }
                }

                if ($.inArray(current, navHideOnSteps) != -1) {
                    anchor.hide();
                    element.addClass("wizard_nav_hidden");
                }
                else {
                    anchor.show();
                    element.removeClass("wizard_nav_hidden");
                }
              

                steps[current].fadeIn();
                //steps[current].css({ opacity: 0.1, display: block }).animate({ opacity: 1 });

                links[states[current]].addClass("selected");

            }

            function setStep(i) {
                current = i;
                defaultState();
                loadStep();
                Show();
                return current
            }

            $.extend(
                wizard,
                {
                    moveToStep: function (index) {
                        return setStep(index)
                    }
                }
            );

            initialise(s);
        }

        settings = $.extend({}, $.fn.JLogicWizard.defaults, settings);

        return new JLogicWizard($(this), settings);

    };

    $.fn.JLogicWizard.defaults = {
        nextButton: false,          // (type: selector) for navigation to left
        prevButton: false,          // (type: selector) for navigation to right
        selected: 0,                // (type: int )     selected step after initialize . Default 0
        navBlock: false,          // (type: selector) selector for steps navigation (selector must contain child tag a - which is considered as step unit)
        content: {                  //                  content param define classes for steps. (Will expand in future)
            steps: [],              // (type: array)    list of steps by index, if empty - all steps is included
            parentClass: false      // (type: selector) class for main widget container
        },
        navHideOnSteps: [],     // (type: array)    list of steps by index, if empty - all steps is included and visible
        stepTitle: {
            text: false,            // (type: string/selector) selector of step block in  which text for title is placed or just string
            output: false           // (type: selector) place for visualization of title text, If text is placed but output.
        },
        validateHandler: false,     // handler for any step validation
        onLoadStepHandler: false,    // handler for actions before step loads.
        onLeaveHandler: false    // handler for actions before step loads.
    };


})(jQuery, this);