namespace System.IO
{
    public static class FileExtensions
    {
        /// <summary>
        /// Renames the file to the specified name
        /// </summary>
        /// <param name="newName">The new name of this file</param>
        /// <param name="replace">Indicates wether to delete the new file path if it exists already</param>
        public static bool Rename(this FileInfo fileInfo, string newName, bool replace = true)
        {
            // First, make sure the file doesnt already exist
            string newPath = Path.Combine(fileInfo.Directory.FullName, newName);
            if (File.Exists(newPath))
            {
                // If we dont want to replace, return false
                if (!replace) 
                    return false;
                else
                    File.Delete(newName);
            }

            // Move file to the new location
            fileInfo.MoveTo(newPath);
            return true;
        }
    }
}
