@file:Repository("https://jitpack.io")
@file:DependsOn("com.github.java-system:java17:2023.0724.0448")

/*
Kotlinでfor文によるループ処理を実装する方法
https://hirauchi-genta.com/kotlin-for/
*/

package app

import system.Sys

fun add3(a: Int, b: Int, c: Int): Int {
    return a + b + c
}

Sys.echo(123)
Sys.echo(add3(11, 22, 33))

for (int i=0; i<10; i++) {

}