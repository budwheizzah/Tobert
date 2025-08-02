var CheckerPlugin = {
        IsMobile: function()
        {
           return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
        }
     };  
mergeInto(LibraryManager.library, CheckerPlugin);