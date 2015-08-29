# unityboot

Spring Boot makes it easy to create Unity based Applications that you can "just make your game".

### Features
> Services
* Service class: Service root class
* sb: string bundle service with multi-language support
* ...
>

> Utils
* ...
>

> Editor Scripts
* Builder: You can make iOS / Android asset bundles
>

### Initialize Services

```c#
class IntroSceneController : MonoBehaviour {
    IEnumerator Start() {
        StartCoroutine(InitializeService());
    }

    IEnumerator InitializeService() {
        Service.ready = false;

        // initialize string bundle
        yield return Service.sb.Initialize("en", "ko");

        // initialize setting service
        yield return Service.Run(Service.setting.Initialize());

        // ...
        Service.ready = true;
    }
}
```
