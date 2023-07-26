@GrabResolver(name='jitpack.io', root='https://jitpack.io')
@Grab('com.github.java-system:java17:2023.0724.0100')

def scriptPath = new File(getClass().protectionDomain.codeSource.location.path).getAbsolutePath()
println scriptPath
def scriptDir = new File(scriptPath).getParentFile().getAbsolutePath()
println scriptDir

println(args)

import system.GroovyVM

var vm = new GroovyVM()
vm.echo(123.45);
System.out.println((123.45).getClass().getName());
vm.echo("this is string!");
vm.eval("vm.echo('hello from groovy')");
vm.eval("vm.echo([1,2,3])");
var a = vm.newList(11, 22, 33);
var x = vm.newMap("a", a);
println x.getClass().getName();
vm.echo(x["a"][1]);
var y = vm.eval("([a: [11, 22, 33]])");
vm.echo(y["a"][1]);
