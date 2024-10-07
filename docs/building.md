# SqlD Help - Building

<div align="right">
	<a href="https://github.com/RealOrko/sql-d/blob/master/docs/_.md#sqld-help---contents">[Back to Contents]</a>
</div>

  * [Building](#building)
  * [Packaging](#packaging)

## Building

<div align="right">
	<a href="#sqld-help---building">[Back to Top]</a>
</div>
<br/>

Builds are currently only supported on linux-x64 platforms. If you have Windows please use WSL/Ubuntu 24.04 and if you are on MacOS you would need a Oracle VirtualBox installation. Please note this script will also execute tests and is used in the main CI/CD build. 

*Ubuntu 24.04*:
```
./scripts/build.sh
```

*See Also*:

  - [About](https://github.com/RealOrko/sql-d/blob/master/docs/about.md)
  - [Prerequisites](https://github.com/RealOrko/sql-d/blob/master/docs/prerequisites.md)

## Packaging

<div align="right">
	<a href="#sqld-help---building">[Back to Top]</a>
</div>
<br/>

You can generate packages locally to test them out. Currently the debian package is tested and will install sqld.ui as a systemd unit. If you would like to try it out please run the commands below.

*Ubuntu 24.04*:
```
./scripts/package.sh
./scripts/install.sh
```

You can then browse locally to `http://localhost:5000` and interact with sql-d using a management UI. 
