# v2.0.0
### Code Cleanup
- Changed the namespace from ```ProphetsWay.Utilities``` to the appropriate ```ProphetsWay.Hasher``` 
  - This was an artifact from when the code was broken out of a single project with many smaller modules.
- Removed the depricated method calls.


# v1.1.0
### Added Algorithms
- Added support for Hashing Algorithms SHA384 and RIPEMD160.  You can now specify them using the HashTypes enum.
- Created a new way to ask for multiple hash checksums against a single stream.  Depricated old methods that returned
HashCollection results.  Those methods will be removed in a new version.
- Created a new VerifyHash method that will figure out which algorithm to use based on the length of the expected hash provided.
If the length is 40 characters, and the runtime is .NetFramework, it will calculate both SHA1 and RIPEMD160 checksums, and compare both
and return true if one matches.  Since RIPEMD160 is unavailable in .NetStandard and .NetCore those versions will only check against SHA1.


# v1.0.1
### Unit Tests Created
Simply added unit tests to the project and removed a few small references


# v1.0.0
### Initial proper release.  
Contains functionality to Hash Streams.  See the README.MD for general information.
