# ArtilleryShootChallenge
Given a cannon, and a field full of targets, each with a different value - what would you(r AI) do?

A programming challenge, originally found here: http://cplus.about.com/od/programmingchallenges/a/programming-challenge-70-artillery-shoot.htm


> This month's challenge is an exercise in optimization. You live in Flatland and are in charge of a cannon that can elevate at angles between 10 and 80 degrees. (Anything near 90 degrees is kind of dangerous under the "What goes up must come down" principle.
> In short, your cannon fires at targets in a two dimensional plane. There are 50 targets worth varying amounts scattered at different distances to the right of your cannon. It only has 10 rounds of ammunition and takes one second to change its angle for every 10 degrees.
> The targets are on a flat piece of land at the same altitude as you so this simplifies the equations. The 50 targets each have a value between 1 and 100 and are scattered randomly in range of your cannon.
> Your job is to write a program that changes the angle of your cannon to get the most points in the shortest total time."

The field is setup, such that 50 targets worth a total of 2200 points are spread out in our (2d) field, randomly placed in a 600m area. Given that we can only hit 110m worth of area in the time allotted, how can we make sure to hit the most targets for the most points?
## The methods
After getting the setup out of the way, I tried three different methods of targetting:

### Basic Point and Shoot
The first attempt. This algorithm scans the target area that can be reached in 1s while the cannon reloads, picking the target with the highest point value as the target.
This worked suprisingly well, yielding 1344 points. But it doesnt take into account collateral damage (the shell explodes, also hitting targets 5m to the left and 5m to the right of the target).

### Clustering
This was a definite improvement in theory. I grouped the targets into clusters 10m wide (like so [0,0,0,0,3,0,0,4,1,0][0,0,4,0,0,0,2,0,0,0][0,0,9,8,0,0,0,1,0,0], etc), then targeted the *cluster* with the highest total point value that could be reached before reloading.
Unfortunately, the performance was dissapointing, a mere 1276 points, less than completely ignoring collateral damage altogether!
This turned out to be a problem with how the clusters were assembled, by looking only at [0,0,0,0,3,0,0,4,1,0][0,0,4,0,0,0,2,0,0,0], we missed the best possible cluster, [3,0,4,1,0,0,0,4,0,0]!

### Higher resolution clustering
The solution turned out to be more clusters. 10x more, to be exact. This way, every possible combination of clusters was represented, including the more optimal ones.
This is more performance intensive though, as the clusters need to be re-computed each time. However, this method worked well, yielding 1427 points, the highest yet!

## Still to come
Theres a lot more improvements that could be made. They mainly center around strategy - right now the cannon computes the optimal shot based on the position it was in after the last shot.
It would be better to pre-compute all 11 shots before hand, and then plan which order to execute them in, so that no ideal shots are missed.

It would also be an interesting exercise to extend the problem further - drop our assumption that the cannon and the targets are at the same elevation (i.e. fire from the top of a hill, hit targets in hills/valleys etc.), or maybe add a 3rd dimension!
