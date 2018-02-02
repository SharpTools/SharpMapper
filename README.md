# SharpMapper
<img src="https://github.com/SharpTools/SharpMapper/raw/master/sharpmapper.png" />
Easy-to-use .net object mapper

[![Build status](https://ci.appveyor.com/api/projects/status/emai3jlku1qihpr4?svg=true)](https://ci.appveyor.com/project/Andre/sharpmapper)

## How to use

SharpMapper will copy all properties from one object to another using the following rules:

 - property is present on target object
 - property name comparison is case insensitive
 - reference objects are created and have to have a parameterless constructor
 - the mapping is recursive following the object tree
 - lists and dictionaries are copied and every object inside them is also created and copied
 - objects are created only once, so if the same object appears in 2 properties of source, a single copy of it will be in the same 2 properties of target too
 - value properties are copied
 - nullable types can be copied to value types
 - null values from source are set null on target object
 - source and target classes can be different (it will only copy the matching properties)
 - **there is no need to pre-create a mapping**
 

### Copy all properties from one object to another
```cs
  var from = new Foo { Name = "foo", Age = 1 };
  var to = Mapper.Map<Foo>(from);
```

### Clone existing object
```cs
  var from = new Foo { Name = "foo", Age = 1 };
  var to = Mapper.Copy(from, to);
```

### Classes don't need to be the same 
```cs
  var from = new Foo { Name = "foo", Age = 1 };
  Bar to = Mapper.Map<Bar>(from);
```
