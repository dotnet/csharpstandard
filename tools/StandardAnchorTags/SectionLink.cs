﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StandardAnchorTags
{
    /// <summary>
    /// Data stored for generating a link to a section.
    /// </summary>
    public readonly struct SectionLink
    {
        // TODO: Add constructor for (removed) section. That would have 
        // no new link, or anchor. TOC markdown link and formatted markdown link should be null (or empty)
        private const char sectionReference = '§';
        public SectionLink(string oldLink, string newLink, string anchor)
        {
            ExistingLinkText = oldLink;
            NewLinkText = newLink;
            AnchorText = anchor;
        }

        /// <summary>
        /// The property is the link text currently used.
        /// </summary>
        /// <remarks>Might not be needed.</remarks>
        public string ExistingLinkText { get; }

        /// <summary>
        /// The text following the § character for any link in the updated standard.
        /// </summary>
        public string NewLinkText { get; }

        /// <summary>
        /// The text string for the destination file and anchor.
        /// </summary>
        public string AnchorText { get; }

        public string FormattedMarkdownLink => $"[{sectionReference}{NewLinkText}]({AnchorText})";

        public string TOCMarkdownLink()
            => $"[{sectionReference}{NewLinkText}]({AnchorText})";
    }
}
