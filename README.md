
# UploadWP
Compresses and uploads the cached version of a Wordpress website


 
 
                      ᓚᘏᗢ
                      
                      
WP Minifier and Uploader
Gets the cached HTML version of a Wordpress website produced by a Wordpress cache plugin and minifies it and uploads it to a web server.
The result is what looks like a Wordpress website that is totally HTML and very light weight. Perfect for a webite this is essentially just a blog and doesn't need constant updates.
It's affectively hack-proof ... as long as your server doesn't get hacked

Really it's kind of like combining Wordpress and Dreamweaver. It was an idea I had years ago but only recently decided it would be a fun project to build.

Jeremy Heminger <contact@jeremyheminger.com>

created May 3, 2023

last update May 25, 2023
 
 - bug     still collecting php files and probably other unwanted extensions
 
   - looks like this is fixed but needs more testing
          
 - bug     seems to be skipping files occasionally 
 
 - todo    program needs to dig through stylesheets to find assets and ( maybe ) javascript - DONE CSS
 
 - todo    connect to WP DB ( might be useful )
 
 - todo    detect theme change
 
 - todo    scan for uploaded orphaned files
 
 
 dependency AQFiles 
 
 dependency AQHelpers 
 
 dependency HTMLAgiltyPack
 
 dependency Newtonsoft.JSON
 
 dependency Renci.SSHNet
 
 
 - version 1.0.0.3
 
	  - feature digs through stylsheets to find assets
  
 - version 1.0.0.2
 
	  - bugfix unwanted file types no longer uploaded
  
 - version 1.0.0.1
 
	  - feature delete removed pages
  
 - version 1.0.0.0
 
 
 
