import json
import random

def generate_combination(number_of_clients, server_rounds, client_epochs, dataset):
    combination = {
        "numberOfClients": number_of_clients,
        "serverRounds": server_rounds,
        "clientEpochs": client_epochs,
        "dataset": dataset,
        "experimentTime": int(100 + 2 * server_rounds + 3 * client_epochs + 10 * random.random()),
        "accuracy": int(60 + 2 * server_rounds + 3 * client_epochs + 10 * random.random()),
        "dataTransmitted": int(10 + 2 * server_rounds + 3 * client_epochs + 10 * (5 if dataset == "fashion-mnist" else 2) * random.random())
    }
    return combination

def generate_combinations(num_combinations):
    combinations = []
    for _ in range(num_combinations):
        number_of_clients = random.randint(1, 10)
        server_rounds = random.randint(1, 10)
        client_epochs = random.randint(1, 10)
        dataset = random.choice(["mnist", "fashion-mnist"])
        
        combination = generate_combination(number_of_clients, server_rounds, client_epochs, dataset)
        combinations.append(combination)

    return combinations

if __name__ == "__main__":
    num_combinations = 500
    combinations = generate_combinations(num_combinations)

    # Save the combinations to a file
    with open("generated_combinations.json", "w") as file:
        json.dump(combinations, file, indent=2)

    print("Combinations saved to 'generated_combinations.json'")

