# Teste Desenvolvedor Unity - Ludact

## Instruções

1 - Fazer naves aparecendo na sequência de fibonacci sem utilizar laços(foreach, for, while, do/while) e utilizando object pooling;

2 - As naves devem possuir velocidades diferentes, aumentando de maneira linear de acordo com o tempo passado;

3 - Fazer 1 nave ser destruída a cada 1 segundo;

4 - Deve haver uma UI demonstrando quantas naves já foram criadas e outras informações que julgar pertinente;

5 - A UI deve ser responsiva com diferentes resoluções;

## Implementação das Instruções

1 - Ship/Spawner/ShipSpawner.cs:28; A minha interpretação dessa instrução foi de fazer a quantidade de naves que aparem na tela serem referentes à sequência de Fibonacci. Outra interpretação que tive, mas descartei foi: o tempo para uma quantidade X qualquer de naves aparecer ser referente à Fibonacci. Achei que do primeiro jeito seria mais interessante. A forma que de fato implementei essa instrução foi usando do próprio Update da Unity e fiz as naves aparecerem em ‘rounds’. A cada round é spawnada uma quantidade de naves Fibonacci(n), utilizando um algoritmo recursivo, onde n é o número de rounds. Como uma nave é destruída a cada segundo, o round muda quando as o número de naves ativas do round chega a zero.

2 - Ship/Spawner/ShipSpawner.cs:49, e Ship/Ship.cs:88; Cada nave é criada com uma velocidade randômica cuja variação é configurável, por default essa variação é de 1 à 10. Além disso sua velocidade é aumentada linearmente por um valor arbitrário também configurável, o padrão sendo 0.05. Por causa do aumento linear, a velocidade chega a valores altos muito rápido e para conseguir mostrar na tela esse aumento fiz com que a naves ficassem confinadas à area da camera.

3 - Destroyer/ShipDestroyer.cs:31; Eu queria que a destruição das naves pudesse ser visualizada, assim, coloquei uma nave estática central cuja função é destruir as outras. A cada segundo a ‘destroyer’ atira em uma nave, quando esta é destruída ela mira em um novo alvo, outra nave, e repete o ciclo.

4 - Além de naves criadas, também adicionei a informação de naves spawnadas (diferente de criadas por causa do reciclamento do object pooling), naves destruídas, naves ativas e informações dos rounds. E também botões que controlam alguns estados do jogo.

5 - A UI é responsiva usando a ancoragem de elementos da Unity.

## Sistemas

Dá pra dividir os sistemas em três áreas maiores: elementos que compõe o jogo, elementos de UI e gerenciamento. Tirando o AudioManager.cs, que é Singleton, os scripts se conversam por eventos, seguindo majoritariamente o Observer design pattern.

### Gameplay

O ‘core loop’ da gameplay é o sistema de rounds, gerenciado pelo próprio spawner das naves ShipSpawner.cs. O spawner possui três variáveis que permitem esse core loop sem utilizar laços, _shipsCounter, _rounds e _fibonacci. A primeira é referente à quantidade de naves spawnadas, isso pra garantir que a cada round a quantidade de naves seja a mesma que a sequência de Fibonacci atual. A segunda conta os rounds, pra ser utilizado como o termo n de Fibonnaci(n), cujo resultado é guardado em _fibonacci.

O spawner monitora a quantidade de objetos do tipo Ship ativas atualmente na pool, e como uma nave é destruída a cada segundo pela nave central (Destroyer), quando a quantidade de naves ativas chega a zero é chamado o método NewRound(). O método reinicia a contagem de _shipsCounter e aumenta em 1 a senquência de Fibonacci.

### Spawners

Além do spawner de naves, também tem o spawner dos tiros e das explosões. Todos funcionam essencialmente da mesma forma, por object pooling. No Start() a pool de objetos é criada e é associada à ela os métodos de evento que serão chamados ao criar um novo objeto (caso não tenha um objeto disponível), ao pegar (Get) um objeto da pool, ao liberá-lo (Release) de volta à ela e ao destruí-lo. A criação é simplesmente uma instanciação, os mais importantes são os métodos de Get e Release que, respectivamente, ativam e desativam o objeto, mantendo sua referência.

### Naves

O funcionamento da nave é de se mover para frente, e aumentar linearmente sua velocidade. A velocidade inicial é definida pelo spawner. Existem dois elementos estáticos que demarcam os limites da área jogável. O Camera Bounds é delimitado pela área da câmera, e o World Bounds é ligeiramente maior que o Camera Bounds. Dito isso, as naves monitoram o estado do GameMode, caso o modo seja ‘Confined’ as naves irão colidir com o Camera Bounds e refletir seu vetor frontal e vetor velocidade dando a impressão de quicar. Caso o modo seja ‘Free’, o GameManager.cs muda a forma de colisão com Camera Bounds para trigger, assim, no último instante de colisão, a nave passa a ser teleportada para trás do limite da câmera porém no lado oposto, dando impressão de que a nave está livre de qualquer limite. Independente do modo de jogo, as naves sempre irão rebater no Destroyer.

### Explosões

O ExplosionSpawner.cs monitora os eventos disparados de quando uma nave é destruída para spawnar uma explosão na posição que aquela nave se encontrava. A explosão em si é simplesmente um sistema de partículas que roda logo ao ser instanciado e retorna para a pool quando acaba a duração.

Tanto as naves quanto as explosões possuem AudioSources embutidos que não são gerenciados pelo AudioManager, por isso, eles, individualmente, monitoram o estado de som e mutam/desmutam quando necessário.

### Destroyer

O ShipDestroyer.cs cumpre a função de destruir uma nave a cada segundo. Seu ciclo de vida é: achar um alvo (quando não existir um ou uma nave for destruída), mirar, atirar (caso um segundo tenha se passado)...

### Tiros

O BulletSpawner.cs é um elemento filho do Destroyer, dessa forma, quando o Destroyer chama o método de atirar, uma Bullet.cs é spawnada e à ela é atribuída o alvo do Destroyer. O funcionamento da Bullet é de simplesmente manter seu vetor frontal alinhado com o alvo e seguir muito rápido para frente.

### Utils

São uma coleção de métodos úteis que posso reusar em qualquer lugar da aplicação. Além do método que calcula a sequência de Fibonacci recursivamente, têm dois métodos para manter os objetos somente nos eixos X e Y, em KeepParentZAxisOf o objeto mantém o eixo Z do parent e em RemoveZAxisOf o eixo Z do objeto é mantido em 0. Esses métodos são necessários as vezes pois a interpolação esférica retorna uma posição nos três eixos. Além disso também existe o método GetRandomPositionInSpawnableArea, que calcula uma posição aleatória dentro da área da câmera.

## UI

A UI é composta de duas áreas principais, o lado esquerdo possui informações, de cima para baixo: a quantidade de naves únicas criadas, a quantidade de naves spawnadas, a quantidade total de naves destruídas, a quantidade de naves que foram spawnadas no round atual, a quantidade de naves atualmente ativas/vivas e em qual round o jogo se encontra; já o lado direito possui botões interativos que mudam estados do jogo, de cima para baixo: pausa e despausa o jogo, muda o modo de jogo (confinar as naves ou deixá-las livres), pula um round, altera o estado de som do jogo (pode-se colocar no mudo apenas os efeitos sonoros, todos os sons ou tirar tudo do mudo) e reinicia o jogo.

### Background

O background possui um efeito parallax, são três imagens em camadas. A camada mais a frente se move mais rápido enquanto a mais atrás se move mais lentamente. A cada uma taxa de segundos configurável (2.5 por default), uma nova posição aleatória é definida para as três camadas seguirem. A velocidade é definida por um interpolação esférica da posição atual da camada e da posição alvo.

## Gerenciamento

### Do Audio

O AudioManager.cs possui uma lista de componentes AudioSource, _tracks, podendo comportar múltiplas faixas ao mesmo tempo e uma lista de sons _sounds. A primeira faixa é separada para música de fundo e a segunda para efeitos sonoros. Os métodos são acessados pela instância estática do AudioManager, o método PlaySoundOneShot toca um som uma vez somente recebendo como parâmetros o número da faixa e do som. Além disso também têm os métodos para colocar ou retirar os sons do mudo.

### Do Jogo

O GameManager.cs possui variáveis estáticas para os estados importantes de jogo, IsGamePaused, se o jogo está atualmente pausado; GameMode, que rege o estado da naves, entre ficarem confinadas à area da câmera (Confined) e ficarem “livres” (Free) e para tal, ele altera a forma de colisão com Camera Bounds para ‘trigger’ quando no modo Free; e SoundState que rege o estado do som, entre deixar mudo somente os SFX (MuteSFX), deixar tudo no mudo (MuteAll), ou tirar tudo do mudo (UnmuteAll). O método que pausa o jogo o faz ao colocar ao mudar o Time.timeScale em 0 (ou 1 para despausar) e, ao pausar, o estado de som também é alterado para MuteAll. Outro método importante é o de SkipRound, ele funciona ao achar todas a naves ativas e destruí-las, assim, o próprio ShipSpawner reconhecerá que um novo round deve ser feito. Além disso, outra função importante do manager é manter o frame rate em 60 fps.

### Da UI

O UIManager.cs mantém a referência dos elementos da interface e os modifica de acordo com os eventos em que é inscrito. Possui variáveis internas para auxiliar principalmente nas contagens, por exemplo, na quantidade de naves criadas ou destruídas, isso para evitar a confusão de parâmetros passados pelos eventos. Um função importante do manager é desabilitar os botões, tirando o de despausar e reinicar, sempre que o jogo é pausado.

## Testes Unitários

Fiz apenas um teste que julguei realmente necessário, que foi verificar se o algoritmo recursivo para o cálculo da sequência de Fibonacci estava correto.

## Créditos

Tentei usar somente os assets que foram providenciados ou que eu mesmo fiz (os sons, o backgrounde e os icons), pra ficar mais autêntico. Porém, a música eu peguei de um
bundle free na Asset Store: [Free Music Bundle by neocrey](https://assetstore.unity.com/packages/audio/music/electronic/free-music-bundle-by-neocrey-92835).
Além disso, o Readme eu vi em um projeto oficial da Unity e achei interessante aplicar aqui, sendo ele o [3D Game Kit](https://assetstore.unity.com/packages/templates/tutorials/3d-game-kit-115747).