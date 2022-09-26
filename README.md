# BioReactor-Revised-

+ made bioreactors rotatable
+ added limited color customization for bioreactors
+ bioreactor histolysis is able to be canceled
+ added basic mod settings
- removed <1.3 support because these changes were not added to those
- removed PublishedFileId.txt because i cannot publish to steam workshop

+ added needs, bio and health tabs for pawns inside bioreactors
+ added allow auto refueling gizmo
- removed fuel consumption when bioreactor is broke down or turned off
* pawns inside bioreactors will now become emancipated when bioreactors are broken down, out of fuel, or turned off and will eventually die from malnutrition
+ pawns are now automatically ejected if they die inside a bioreactor
+ added alert letters for when bioreactors break down, run out of fuel or are turned off while they contain a pawn
- removed restriction that prevented mechanoids from being placed into bioreactors
* rewrote mod settings to use Widgets calls instead of a Listing_Standard object
+ filled suggestion/request from [ #1 ](https://github.com/solaris0115/BioReactor/issues/1), max body size is now listed in the information window (the "i" button)

issue: during testing i encountered an error that had something to do with reserving pawns when there are multiple opened bioreactors, but i havent been able to reproduce this.
