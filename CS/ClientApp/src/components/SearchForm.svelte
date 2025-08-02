<form action="/search" method="get">
    <div class="search-bar">
        <input name="q" placeholder="Enter a word to search">
    </div>
    <div>
        <input id="Noun" name="pos" type="radio" value="any" checked/>
        <label for="noun">Any</label>
        <input id="Noun" name="pos" type="radio" value="noun" />
        <label for="noun">Noun</label>
        <input id="verb" name="pos"type="radio" value="verb" />
        <label for="verb">Verb</label>
        <input id="adjective" name="pos" type="radio" value="adjective" />
        <label for="adjective">Adjective</label>
    </div>
</form>

<style>
    .search-bar {
        display: flex;
        align-items: flex-start;
        margin-bottom: 5px;
    }
    input {
        font-size: 2em;
        flex: 1;
    }
    input[type="radio"] {
        display: inline-block;
        vertical-align:middle;
    }
</style>
