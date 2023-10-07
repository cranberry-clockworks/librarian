<script>
    export let article = ""
    export let lemma = "Lemma"
    export let wordClass = "Unknown"
</script>

<span class="lemma">{article.length > 0 ? article + " " : ""}{lemma}</span>
<span class="wordClass">{wordClass}</span>

<style>
    .lemma {
        font-size: xx-large;
        margin-right: 10px;
    }
    .wordClass {
        text-transform: uppercase;
    }
</style>