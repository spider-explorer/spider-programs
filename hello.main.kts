@file:Repository("https://jitpack.io")
@file:DependsOn("com.github.java-system:java17:2023.0724.0448")

import org.w3c.dom.Element
import org.w3c.dom.NodeList
import system.Sys
import java.io.FileInputStream
import java.nio.file.Paths
import javax.xml.parsers.DocumentBuilderFactory
import javax.xml.xpath.XPathConstants
import javax.xml.xpath.XPathFactory

Sys.echo(123.45)
var obj = Sys.readAsJson("00-software.json")
Sys.echo(obj, "obj")
Sys.echoJson(obj, "obj")

// 1.Documentを作るまでの流れはDOMと同じ

// 1.Documentを作るまでの流れはDOMと同じ
var `is` = FileInputStream(Paths.get("bookList.xml").toFile())
var factory = DocumentBuilderFactory.newInstance()
var builder = factory.newDocumentBuilder()
var document = builder.parse(`is`)

// 2.XPathの処理を実行するXPathのインスタンスを取得する

// 2.XPathの処理を実行するXPathのインスタンスを取得する
var xpath = XPathFactory.newInstance().newXPath()
// 3.XPathでの検索条件を作る
// 3.XPathでの検索条件を作る
var expr = xpath.compile("/BookList/Book")
// 4.DocumentをXPathで検索して、結果をDOMのNodeListで受け取る
// 4.DocumentをXPathで検索して、結果をDOMのNodeListで受け取る
var nodeList = expr.evaluate(document, XPathConstants.NODESET) as NodeList

// 5.XPathでの検索結果を持っているNodeListの内容でループ

// 5.XPathでの検索結果を持っているNodeListの内容でループ
for (i in 0 until nodeList.length) {
    // 6.要素を検索しているのでNodeの実体はElement。キャストして使う。
    val element = nodeList.item(i) as Element

    // 7.Elementから必要な情報を取得して出力する
    println("isbn = " + element.getAttribute("isbn"))
    println("title = " + element.getAttribute("title"))
    println("author = " + element.getAttribute("author"))
    println("text = " + element.textContent)
    println()
}

