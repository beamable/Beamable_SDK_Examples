using System;
using System.Collections;
using System.Collections.Generic;
using Beamable.Common;
using UnityEngine;
using UnityEngine.Assertions;

namespace Beamable.Examples.LearningFundamentals.PromiseProgramming
{
    /// <summary>
    /// Demonstrates <see cref="Promise"/> programming.
    /// </summary>
    public class PromiseProgrammingExample : MonoBehaviour
    {
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log("Start()");

            Debug.Log("1 StringPromiseWithCompleteSuccess()");
            StringPromiseWithCompleteSuccess();
            
            //Debug.Log("2 StringPromiseWithCompleteError()");
            //StringPromiseWithCompleteError();
            
            Debug.Log("3 PromiseSequenceWithCompleteSuccess()");
            PromiseSequenceWithCompleteSuccess();
            
            Debug.Log("4 PromiseBatchWithCompleteSuccess()");
            PromiseBatchWithCompleteSuccess();
            
            Debug.Log("5 StringPromiseCoroutineWithCompleteSuccess()");
            StringPromiseCoroutineWithCompleteSuccess();
            
        }
        
        
        //  Methods  (BEGINNER) ---------------------------
        
        
        private void StringPromiseWithCompleteSuccess()
        {
            // 1. Arrange
            var stringPromise = new Promise<string>();
            
            // 3. Assert
            stringPromise.Then(result =>
            {
                Debug.Log($"stringPromise.Then() result = {result}");
                Assert.AreEqual(result, "Hello World!");
            });

            // 2. Act
            stringPromise.CompleteSuccess("Hello World!");
        }
        
        
        private void StringPromiseWithCompleteError()
        {
            // 1. Arrange
            var stringPromise = new Promise<string>();
            
            // 3. Assert (Exception will be thrown)
            stringPromise.Then(result =>
            {
                Debug.Log($"stringPromise.Then() result = {result}");
                Assert.AreEqual(result, "Hello World!");
            });

            // 2. Act
            stringPromise.CompleteError(new Exception("Something went wrong before result is ready"));
        }
        
        
        //  Methods  (ADVANCED) ---------------------------
        private void PromiseSequenceWithCompleteSuccess()
        {
            // 1. Arrange
            var stringPromise1 = new Promise<string>();
            var stringPromise2 = new Promise<string>();
            var promiseSequence = Promise.Sequence(new List<Promise<string>> {stringPromise1, stringPromise2});
            
            // 3. Assert
            promiseSequence.Then(result =>
            {
                Debug.Log($"promiseSequence.Then() result = {result.Count}");
                Assert.AreEqual(result[0], "Hello World!");
                Assert.AreEqual(result[1], "Foo Bar");
            });

            // 2. Act
            stringPromise1.CompleteSuccess("Hello World!");
            stringPromise2.CompleteSuccess("Foo Bar");
        }
        
        
        private void PromiseBatchWithCompleteSuccess()
        {
            // 2. Act
            Func<Promise<Unit>> SomethingReturningPromise1()
            {
                return () =>
                {
                    var stringPromise1 = new Promise<Unit>();
                    stringPromise1.CompleteSuccess(new Unit());
                    return stringPromise1;
                };
            }
            Func<Promise<Unit>> SomethingReturningPromise2()
            {
                return () =>
                {
                    var stringPromise2 = new Promise<Unit>();
                    stringPromise2.CompleteSuccess(new Unit());
                    return stringPromise2;
                };
            }

            // 1. Arrange
            var promiseList = new List<Func<Promise<Unit>>>();
            promiseList.Add(SomethingReturningPromise1());
            promiseList.Add(SomethingReturningPromise2());
            var promiseBatch = Promise.ExecuteInBatch(10, promiseList);
            
            // 3. Assert
            promiseBatch.Then(result =>
            {
                Debug.Log($"promiseBatch.Then() result = {result}");
            });

        }
        
        
        private void StringPromiseCoroutineWithCompleteSuccess()
        {
            StartCoroutine(StringPromiseCoroutine());
        }
        
        
        private IEnumerator StringPromiseCoroutine()
        {
            // 1. Arrange
            var stringPromise = new Promise<string>();
            
            // 3. Assert
            stringPromise.Then(result =>
            {
                Debug.Log($"stringPromise.Then() result = {result}");
                Assert.AreEqual(result, "Hello World!");
            });

            // 2. Act
            stringPromise.CompleteSuccess("Hello World!");
            
            yield return stringPromise.ToYielder();
            
        }
        
        //  Event Handlers  -------------------------------
    }
}