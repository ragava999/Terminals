using System;
using System.Collections.Generic;

namespace Terminals.Configuration.Files.Main.Tags
{
    /// <summary>
    ///     Tags changed event arguments container, informing about changes in Tags collection
    /// </summary>
    public class TagsChangedArgs : EventArgs
    {
        public TagsChangedArgs()
        {
            this.Added = new List<String>();
            this.Removed = new List<String>();
        }

        public TagsChangedArgs(List<String> addedTags, List<String> deletedTags)
        {
            // merge collections to report only difference
            MergeChangeLists(addedTags, deletedTags);
            this.Added = addedTags;
            this.Removed = deletedTags;
        }

        /// <summary>
        ///     All tags actualy no longer used by any favorite
        /// </summary>
        public List<String> Added { get; private set; }

        /// <summary>
        ///     Newly added tags, currently used atleast by one connection
        /// </summary>
        public List<String> Removed { get; private set; }

        /// <summary>
        ///     Gets the value idicating if there are any added or reomoved items to report.
        /// </summary>
        public Boolean IsEmpty
        {
            get { return this.Added.Count == 0 && this.Removed.Count == 0; }
        }

        private static void MergeChangeLists(List<String> addedTags, List<String> deletedTags)
        {
            int index = 0;
            while (index < deletedTags.Count)
            {
                String deletedTag = deletedTags[index];
                if (addedTags.Contains(deletedTag))
                {
                    addedTags.Remove(deletedTag);
                    deletedTags.Remove(deletedTag);
                }
                else
                    index++;
            }
        }

        public override String ToString()
        {
            return String.Format("TagsChangedArgs:Added={0};Removed={1}",
                                 this.Added.Count, this.Removed.Count);
        }
    }
}