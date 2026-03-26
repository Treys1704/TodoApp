import { useTodosQuery } from "./hooks/useTodos";
import Header from "./components/Header";
import TodoForm from "./components/TodoForm";
import FilterBar from "./components/FilterBar";
import TodoList from "./components/TodoList";

function App() {
  const { data: todos } = useTodosQuery();

  const totalCount = todos?.length ?? 0;
  const completedCount = todos?.filter((t) => t.isCompleted).length ?? 0;

  return (
    <div className="min-h-screen bg-stone-50">
      <div className="max-w-lg mx-auto px-5 pt-16 pb-24">
        <Header totalCount={totalCount} completedCount={completedCount} />
        <div className="mt-8 bg-white rounded-2xl shadow-[0_1px_3px_rgba(0,0,0,0.04)] border border-stone-200/60 p-5">
          <TodoForm />
          <FilterBar />
          <TodoList />
        </div>
      </div>
    </div>
  );
}

export default App;
