import { GUID  } from './Guid';

describe('Static GUID class', () => {

    describe("GUID.GeneratGuid", () => {

        test("should return a string", () => {
            const newGuid = GUID.GeneratGuid();
            expect(typeof newGuid).toBe("string");
        });

        test("should return a valid GUID format", () => {
            // inspired from: https://stackoverflow.com/a/13653180
            const validGuidRegex =
              /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
            const newGuid = GUID.GeneratGuid();
            expect(newGuid).toMatch(validGuidRegex);
          });
      
        test("should create unique GUIDs", () => {
            const guid1 = GUID.GeneratGuid();
            const guid2 = GUID.GeneratGuid();
            expect(guid1).not.toBe(guid2);
        });
    });
});
