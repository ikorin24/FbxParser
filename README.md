# FBX Parser

[![GitHub license](https://img.shields.io/github/license/ikorin24/FbxParser?color=009DFF)](https://github.com/ikorin24/FbxParser/blob/master/LICENSE)
[![nuget](https://img.shields.io/badge/nuget-v2.0.0-009DFF)](https://www.nuget.org/packages/FbxParser)

FBX file (`.fbx`) low-level Parser in C#.

FBX is just a hierarchical structual format like json(`.json`), xml(`.xml`), and so on.

## Supported Format Version

`.fbx` file has some format version.

- [x] ver7.4
- [x] ver7.5
- [ ] Any other versions (***NOT SUPPORTED of this parser***)

## How to Use

Parsing is very easy.

```cs
using System.IO;
using FbxTools;

using var stream = File.OpenRead("your_file.fbx");
using var fbxObj = FbxParser.Parse(stream);
// Use fbx object here.
// Its memory is released when disposed.
```

## License

This is licensed under MIT.

## Author

ikorin24 ([github](https://github.com/ikorin24))
