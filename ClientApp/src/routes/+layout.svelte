<script>
    import CardList from "../components/cards/CardList.svelte";
</script>

<div class="row">
    <div class="column left">
        <CardList/>
    </div>
    
    <div class="column right">
        <nav>
            <form action="/search" method="get">
                <input name="q" placeholder="Enter a word to search">
            </form>
        </nav>
        <slot/>
    </div>
</div>


<style>
    input {
        font-size: 2em;
        width: 100%;
    }
    
    .row {
        display: flex;
    }
    .column {
        float: left;
        margin-left: 10px;
    }
    .left {
        width: 25%;
        
    }
    .right {
        width: 75%;
    }
    .row:after {
        content: "";
        display: table;
        clear: both;
    }
</style>

