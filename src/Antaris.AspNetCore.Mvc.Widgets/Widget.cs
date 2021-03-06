﻿namespace Antaris.AspNetCore.Mvc.Widgets
{
    using System.Security.Principal;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Antaris.AspNetCore.Mvc.Widgets.Infrastructure;

    /// <summary>
    /// Provides a base implementation of a widget.
    /// </summary>
    [Widget]
    public abstract class Widget
    {
        private IUrlHelper _url;
        private dynamic _viewBag;
        private WidgetContext _widgetContext;
        private ICompositeViewEngine _viewEngine;

        /// <summary>
        /// Gets the HTTP context.
        /// </summary>
        public HttpContext HttpContext => ViewContext?.HttpContext;

        /// <summary>
        /// Gets the HTTP request.
        /// </summary>
        public HttpRequest HttpRequest => HttpContext?.Request;

        /// <summary>
        /// Gets the model state.
        /// </summary>
        public ModelStateDictionary ModelState => ViewContext?.ModelState;

        /// <summary>
        /// Gets the route data.
        /// </summary>
        public RouteData RouteData => ViewContext?.RouteData;

        /// <summary>
        /// Gets or sets the URL helper.
        /// </summary>
        public IUrlHelper Url
        {
            get
            {
                if (_url == null)
                {
                    _url = HttpContext?.RequestServices?.GetRequiredService<IUrlHelper>();
                }

                return _url;
            }
            set
            {
                _url = value;
            }
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        public IPrincipal User => HttpContext?.User;

        /// <summary>
        /// Gets the view bag.
        /// </summary>
        public dynamic ViewBag
        {
            get
            {
                if (_viewBag == null)
                {
                    _viewBag = new DynamicViewData(() => ViewData);
                }

                return _viewBag;
            }
        }

        /// <summary>
        /// Gets the view context.
        /// </summary>
        public ViewContext ViewContext => WidgetContext.ViewContext;

        /// <summary>
        /// Gets the view data dictionary.
        /// </summary>
        public ViewDataDictionary ViewData => ViewContext?.ViewData;

        /// <summary>
        /// Gets or sets the view engine.
        /// </summary>
        public ICompositeViewEngine ViewEngine
        {
            get
            {
                if (_viewEngine == null)
                {
                    _viewEngine = HttpContext?.RequestServices?.GetRequiredService<ICompositeViewEngine>();
                }

                return _viewEngine;
            }
            set
            {
                _viewEngine = value;
            }
        }

        /// <summary>
        /// Gets or sets the widget context.
        /// </summary>
        [WidgetContext]
        public WidgetContext WidgetContext
        {
            get
            {
                if (_widgetContext == null)
                {
                    _widgetContext = new WidgetContext();
                }

                return _widgetContext;
            }
            set
            {
                _widgetContext = value;
            }
        }

        /// <summary>
        /// Returns a new <see cref="HtmlContentWidgetResult"/>.
        /// </summary>
        /// <param name="content">The content to write.</param>
        /// <returns>The widget result instance.</returns>
        public ContentWidgetResult Content(string content)
        {
            return new ContentWidgetResult(content);
        }

        /// <summary>
        /// Returns a new <see cref="ViewWidgetResult"/> for rendering a widget view.
        /// </summary>
        /// <returns>The widget result instance.</returns>
        public ViewWidgetResult View()
        {
            return View<object>(null, null);
        }

        /// <summary>
        /// Returns a new <see cref="ViewWidgetResult"/> for rendering a widget view.
        /// </summary>
        /// <param name="viewName">The view name.</param>
        /// <returns>The widget result instance.</returns>
        public ViewWidgetResult View(string viewName)
        {
            return View<object>(viewName, null);
        }


        /// <summary>
        /// Returns a new <see cref="ViewWidgetResult"/> for rendering a widget view.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="model">The model instance.</param>
        /// <returns>The widget result instance.</returns>
        public ViewWidgetResult View<T>(T model)
        {
            return View<T>(null, model);
        }

        /// <summary>
        /// Returns a new <see cref="ViewWidgetResult"/> for rendering a widget view.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="viewName">The view name.</param>
        /// <param name="model">The model instance.</param>
        /// <returns>The widget result instance.</returns>
        public ViewWidgetResult View<T>(string viewName, T model)
        {
            var viewData = new ViewDataDictionary<T>(ViewData, model);

            // MA - We need to flow the widget context here so it is available in tag helpers.
            viewData.Add(WidgetConstants.WidgetContextKey, WidgetContext);

            return new ViewWidgetResult()
            {
                ViewEngine = ViewEngine,
                ViewName = viewName,
                ViewData = viewData
            };
        }
    }
}