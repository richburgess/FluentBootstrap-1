﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FluentBootstrap
{
    public static class ContentExtensions
    {
        public static ComponentBuilder<TConfig, Content> Content<TConfig, TComponent>(this BootstrapHelper<TConfig, TComponent> helper, object content)
            where TConfig : BootstrapConfig
            where TComponent : Component, ICanCreate<Content>
        {
            return new ComponentBuilder<TConfig, Content>(helper.Config, new Content(helper, content));
        }
    }
}
