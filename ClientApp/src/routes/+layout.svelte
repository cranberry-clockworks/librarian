<script>
    import CardList from "../components/CardList.svelte";
</script>

<div class="side">
    <CardList/>
</div>

<div class="main">
    <nav>
        <form action="/search" method="get">
            <input name="q" placeholder="Enter a word to search">
        </form>
    </nav>
    <slot/>
</div>


<style>
    input {
        font-size: 2em;
        width: 100%;
    }
    .side {
        width: 200px;
        position: fixed;
        top: 0;
        left: 0;
        height: 100%;
        overflow: auto;
        padding-left: 20px;
    }
    .main {
        margin-left: 200px;
        
    }
</style>

