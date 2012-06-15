/**
                 * Takes a URI and formats it according to the prefix map.
                 * This basically is a fire and forget function, punch in 
                 * full uris, prefixed uris or anything and it will be fine
                 * 
                 * 1. if uri can be prefixed, prefixes it and returns
                 * 2. checks whether uri is already prefixed and returns
                 * 3. else it puts brackets around the <uri>
                 */
                private function uri(uri:String):String {
                        
                        //Prefixe und Sonderzeichen funktionieren nicht zusammen!!!
                        
                        //for (var key:String in prefixes) {
                                //if (startsWith(uri, prefixes[key] )) {
                                        //uri = uri.replace(prefixes[key], key + ":");
                                        //return uri;
                                //}
                        //}
                        //
                        //for (var key2:String in prefixes) {
                                //if (startsWith(uri,  (key2 + ":") )) {
                                        //return uri;
                                //}
                        //}
                        return "<" + uri + ">";
                }