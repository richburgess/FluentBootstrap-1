﻿using FluentBootstrap.Grids;
using FluentBootstrap.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBootstrap.Forms
{
    public interface IFormGroupCreator<TModel> : IComponentCreator<TModel>
    {
    }

    public class FormGroupWrapper<TModel> : TagWrapper<TModel>,
        ILabelCreator<TModel>,
        IFormControlCreator<TModel>,
        IHelpBlockCreator<TModel>
    {
    }

    internal interface IFormGroup : ITag
    {
        ILabel Label { set; }
    }

    public class FormGroup<TModel> : Tag<TModel, FormGroup<TModel>, FormGroupWrapper<TModel>>, IFormGroup, IHasGridColumnExtensions, IFormValidation
    {
        private ILabel _label = null;
        private Element<TModel> _columnWrapper;
        private bool _columnWrapperBeforeLabel = false;

        internal ILabel Label
        {
            set { _label = value; }
        }

        ILabel IFormGroup.Label
        {
            set { Label = value; }
        }

        internal bool HasLabel
        {
            get { return _label != null; }
        }

        internal bool? Horizontal { get; set; }

        internal FormGroup(IComponentCreator<TModel> creator)
            : base(creator, "div", Css.FormGroup)
        {

        }
        
        protected override void OnStart(TextWriter writer)
        {
            // Set column classes if we're horizontal          
            IForm form = GetComponent<IForm>();
            if ((form != null && form.Horizontal && (!Horizontal.HasValue || Horizontal.Value)) || (Horizontal.HasValue && Horizontal.Value))
            {
                int labelWidth = form == null ? Bootstrap.DefaultFormLabelWidth : form.DefaultLabelWidth;

                // Set label column class
                if (_label != null && !_label.CssClasses.Any(x => x.StartsWith("col-")))
                {
                    _label.SetColumnClass("col-md-", labelWidth);
                }

                // Add column classes to this (these will get moved to a wrapper later in this method)
                if (!CssClasses.Any(x => x.StartsWith("col-")))
                {
                    this.SetMd(Bootstrap.GridColumns - labelWidth);

                    // Also need to add an offset if no label
                    if (_label == null)
                    {
                        this.SetMdOffset(labelWidth);
                    }
                }
            }
            else if (form != null && form.Horizontal)
            {
                // If the form is horizontal but we requested not to be, create a full-width column wrapper
                this.SetMd(Bootstrap.GridColumns);
                _columnWrapperBeforeLabel = true;
            }

            // Move any grid column classes to a container class
            if (CssClasses.Any(x => x.StartsWith("col-")))
            {
                _columnWrapper = new Element<TModel>(Helper, "div", CssClasses.Where(x => x.StartsWith("col-")).ToArray());
            }
            CssClasses.RemoveWhere(x => x.StartsWith("col-"));

            base.OnStart(writer);

            // Write the column wrapper first if needed
            if (_columnWrapperBeforeLabel && _columnWrapper != null)
            {
                _columnWrapper.Start(writer);
            }

            // Write the label
            if (_label != null)
            {
                _label.StartAndFinish(writer);
            }

            // Write the column wrapper
            if (!_columnWrapperBeforeLabel && _columnWrapper != null)
            {
                _columnWrapper.Start(writer);
            }
        }

        protected override void OnFinish(TextWriter writer)
        {
            Pop(_columnWrapper, writer);
            base.OnFinish(writer);
        }
    }
}
